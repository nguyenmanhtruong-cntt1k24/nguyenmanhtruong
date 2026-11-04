using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class EquipmentService(HpuniDbContext db, IAccessService access, AdminService admin, IdempotentCommands commands)
{
    private static readonly string[] Holding = ["ready", "receipt_disputed", "borrowed", "return_pending"];
    private static bool Holds(EquipmentLoan r) => Holding.Contains(r.Status);
    private static string Note(string? value) => AdminService.OptionalNote(value);
    private async Task Demand(string actor, string permission, string department, string? owner, CancellationToken ct)
    {
        if (!AccessRules.Allows(await access.GetProfile(actor, ct), permission, department, owner)) throw AppException.Forbidden();
    }
    public async Task Directory(string actor, CancellationToken ct)
    {
        var p = await access.GetProfile(actor, ct);
        if (!AccessRules.HasPermission(p, "devices.borrow") && !AccessRules.HasPermission(p, "resources.manage") && !AccessRules.HasPermission(p, "requests.review"))
            throw AppException.Forbidden();
    }
    private async Task<AssetGroup> Asset(string id, CancellationToken ct) =>
        await db.AssetGroups.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();

    public async Task<AssetGroupView[]> AssetViews(AssetGroup[] rows, CancellationToken ct)
    {
        var ids = rows.Select(x => x.Id).ToArray();
        var holding = await db.EquipmentLoans.AsNoTracking().Where(x => ids.Contains(x.AssetGroupId) && Holding.Contains(x.Status)).ToArrayAsync(ct);
        var now = DateTimeOffset.UtcNow;
        var maintenance = await db.MaintenanceOrders.AsNoTracking().Where(x=>ids.Contains(x.AssetGroupId) && x.Mode=="service" && (x.Status=="running" || x.Status=="planned" && x.Start<=now && x.End>now)).ToArrayAsync(ct);
        return rows.Select(a => {
            var outstanding = holding.Where(x => x.AssetGroupId == a.Id).Sum(x => x.Quantity - x.Returned);
            var blocked = maintenance.Where(x=>x.AssetGroupId==a.Id).Sum(x=>x.Quantity);
            return new AssetGroupView(a.Id, a.Code, a.Name, a.Category, a.Unit, a.DepartmentId, a.Total, a.Damaged, a.Lost,
                outstanding, Math.Max(0, a.Total - a.Damaged - a.Lost - outstanding - blocked), a.Active, Convert.ToBase64String(a.RowVersion), blocked);
        }).ToArray();
    }

    public Task<AssetGroupView> CreateAsset(string actor, string key, SaveAssetGroup input, CancellationToken ct) =>
        commands.Run(actor, key, "equipment.create", input,
            () => Demand(actor, "resources.manage", input.DepartmentId, null, ct), async () => {
                await TransactionLocks.Acquire(db, "equipment-catalog", ct);
                if (!await db.Departments.AnyAsync(x => x.Id == input.DepartmentId && x.Active, ct)) throw AppException.Invalid("Đơn vị không hợp lệ.");
                var a = new AssetGroup { DepartmentId = input.DepartmentId };
                Assign(a, input); db.AssetGroups.Add(a);
                admin.Audit(actor, "equipment.create", a.Id, new { a.Code, a.DepartmentId });
                await db.SaveChangesAsync(ct);
                return (await AssetViews([a], ct))[0];
            }, ct);

    public async Task<AssetGroupView> UpdateAsset(string actor, string id, SaveAssetGroup input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "asset:" + id, ct);
        var a = await db.AssetGroups.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        await Demand(actor, "resources.manage", a.DepartmentId, null, ct);
        if (input.DepartmentId != a.DepartmentId) throw AppException.Invalid("Chưa hỗ trợ chuyển đơn vị sở hữu tài sản.");
        AdminService.Version(a.RowVersion, input.Version);
        if (!input.Active && await db.MaintenanceOrders.AnyAsync(x=>x.AssetGroupId==id && (x.Status=="planned" || x.Status=="running"),ct))
            throw AppException.Invalid("Tài sản còn kế hoạch bảo trì hiệu lực.");
        if (!input.Active && await db.EquipmentLoans.AnyAsync(x => x.AssetGroupId == id &&
            (Holding.Contains(x.Status) || (x.Status == "pending" || x.Status == "approved") && x.End > DateTimeOffset.UtcNow), ct))
            throw AppException.Invalid("Tài sản còn phiếu hiệu lực, chưa thể ngừng cho mượn.");
        var before = new { a.Code, a.Name, a.Active };
        Assign(a, input);
        admin.Audit(actor, "equipment.update", id, new { before, after = input });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return (await AssetViews([a], ct))[0];
    }
    private static void Assign(AssetGroup a, SaveAssetGroup input)
    {
        a.Code = AdminService.Required(input.Code, 30).ToUpperInvariant();
        a.Name = AdminService.Required(input.Name, 150);
        a.Category = AdminService.Required(input.Category, 100);
        a.Unit = AdminService.Required(input.Unit, 30);
        a.Active = input.Active;
    }
    public Task<AssetGroupView> Receive(string actor, string id, string key, ReceiveStock input, CancellationToken ct) =>
        commands.Run(actor, key, "equipment.stock:" + id, input, async () => {
            var a = await Asset(id, ct); await Demand(actor, "resources.manage", a.DepartmentId, null, ct);
        }, async () => {
            await TransactionLocks.Acquire(db, "asset:" + id, ct);
            var a = await db.AssetGroups.SingleAsync(x => x.Id == id, ct);
            await Demand(actor, "resources.manage", a.DepartmentId, null, ct);
            AdminService.Version(a.RowVersion, input.Version);
            if (input.Quantity is < 1 or > 1000000 || (long)a.Total + input.Quantity > 1000000) throw AppException.Invalid("Tổng số lượng tối đa 1.000.000.");
            a.Total += input.Quantity;
            Movement(actor, a.Id, null, "stock.received", input.Quantity, input.Note);
            admin.Audit(actor, "equipment.stock", id, new { input.Quantity, note = Note(input.Note), a.Total });
            await db.SaveChangesAsync(ct);
            return (await AssetViews([a], ct))[0];
        }, ct);

    public async Task<long> Free(AssetGroup a, DateTimeOffset start, DateTimeOffset end, string? exclude, CancellationToken ct, string? excludeMaintenance = null)
    {
        var rows = await db.EquipmentLoans.AsNoTracking().Where(x => x.AssetGroupId == a.Id && x.Id != exclude &&
            (x.Status == "pending" || x.Status == "approved" || Holding.Contains(x.Status))).ToArrayAsync(ct);
        var spans = rows.Select(x => new QuantitySpan(Holds(x) && x.HandedAt < x.Start ? x.HandedAt.Value : x.Start,
            Holds(x) ? DateTimeOffset.MaxValue : x.End, x.Quantity - x.Returned));
        var maintenance = await db.MaintenanceOrders.AsNoTracking().Where(x=>x.AssetGroupId==a.Id && x.Id!=excludeMaintenance && x.Mode=="service" && (x.Status=="planned" || x.Status=="running")).ToArrayAsync(ct);
        spans = spans.Concat(maintenance.Select(x=>new QuantitySpan(x.Start,x.Status=="running"?DateTimeOffset.MaxValue:x.End,x.Quantity)));
        return a.Total - a.Damaged - a.Lost - EquipmentCapacity.Peak(spans, start, end);
    }
    public async Task<object> Availability(string actor, string id, DateTimeOffset start, DateTimeOffset end, CancellationToken ct)
    {
        await Directory(actor, ct); ValidateInterval(start, end, false);
        var a = await Asset(id, ct);
        if (!a.Active) throw AppException.Missing();
        return new { assetGroupId = id, start, end, available = Math.Max(0, await Free(a, start, end, null, ct)) };
    }
    private static void ValidateInterval(DateTimeOffset start, DateTimeOffset end, bool future = true)
    {
        if (start >= end || end - start > TimeSpan.FromDays(30) || future && start <= DateTimeOffset.UtcNow)
            throw AppException.Invalid("Lịch mượn phải ở tương lai, kết thúc sau bắt đầu, tối đa 30 ngày.");
    }
    private async Task ValidateLoan(EquipmentLoan r, AssetGroup a, bool future, CancellationToken ct)
    {
        ValidateInterval(r.Start, r.End, future);
        if (!a.Active || r.Quantity is < 1 or > 1000000 || r.Quantity > a.Total - a.Damaged - a.Lost)
            throw AppException.Invalid("Thiết bị ngừng cho mượn hoặc số lượng không hợp lệ.");
        if (await Free(a, r.Start, r.End, r.Id, ct) < r.Quantity)
            throw new AppException(409, "INSUFFICIENT_EQUIPMENT", "Không đủ thiết bị trong khoảng đăng ký. Vui lòng chọn lại.");
    }
    public Task<EquipmentLoanView> Create(string actor, string key, SaveEquipmentLoan input, CancellationToken ct) =>
        commands.Run(actor, key, "equipment-loan.create", input, async () => {
            var p = await access.GetProfile(actor, ct); await Demand(actor, "devices.borrow", p.DepartmentId, actor, ct);
        }, async () => {
            await TransactionLocks.Acquire(db, "asset:" + input.AssetGroupId, ct);
            var p = await access.GetProfile(actor, ct);
            var a = await Asset(input.AssetGroupId, ct);
            var r = new EquipmentLoan { AssetGroupId = a.Id, OwnerId = actor, DepartmentId = p.DepartmentId,
                Start = input.Start.ToUniversalTime(), End = input.End.ToUniversalTime(), Quantity = input.Quantity, Purpose = Note(input.Purpose) };
            ValidateInterval(r.Start, r.End);
            if (!a.Active || r.Quantity < 1 || r.Quantity > a.Total - a.Damaged - a.Lost) throw AppException.Invalid("Số lượng hoặc thiết bị không hợp lệ.");
            db.EquipmentLoans.Add(r); admin.Audit(actor, "equipment-loan.create", r.Id, input);
            await db.SaveChangesAsync(ct); return await View(r, a, ct);
        }, ct);

    public async Task<EquipmentLoanView> UpdateDraft(string actor, string id, SaveEquipmentLoan input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var known = await Get(actor, id, false, ct);
        if (input.AssetGroupId != known.AssetGroupId) throw AppException.Invalid("Không đổi thiết bị trên phiếu đã tạo.");
        await TransactionLocks.Acquire(db, "asset:" + known.AssetGroupId, ct);
        var r = await db.EquipmentLoans.SingleAsync(x => x.Id == id, ct);
        await Get(actor, id, false, ct);
        AdminService.Version(r.RowVersion, input.Version);
        State(r, "draft");
        var a = await Asset(r.AssetGroupId, ct);
        r.Start = input.Start.ToUniversalTime(); r.End = input.End.ToUniversalTime();
        r.Quantity = input.Quantity; r.Purpose = Note(input.Purpose);
        ValidateInterval(r.Start, r.End);
        if (!a.Active || r.Quantity < 1 || r.Quantity > a.Total - a.Damaged - a.Lost)
            throw AppException.Invalid("Số lượng hoặc thiết bị không hợp lệ.");
        admin.Audit(actor, "equipment-loan.update", id, new { r.Start, r.End, r.Quantity });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
        return await View(r, a, ct);
    }

    public async Task<EquipmentLoan> Get(string actor, string id, bool review, CancellationToken ct)
    {
        var r = await db.EquipmentLoans.AsNoTracking().Include(x => x.Asset).SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        var p = await access.GetProfile(actor, ct);
        var allowed = review ? r.Status != "draft" &&
            (AccessRules.Allows(p, "requests.review", r.DepartmentId, r.OwnerId) || AccessRules.Allows(p, "resources.manage", r.Asset.DepartmentId, null))
            : r.OwnerId == actor && AccessRules.Allows(p, "devices.borrow", r.DepartmentId, r.OwnerId);
        if (!allowed) throw AppException.Missing();
        return r;
    }
    public IQueryable<EquipmentLoan> VisibleLogs(AccessProfile p)
    {
        if(!AccessRules.HasPermission(p,"logs.review"))throw AppException.Forbidden();
        var deps=p.DepartmentIds.ToArray();
        var all=p.Grants.Any(g=>g.Permissions.Contains("logs.review")&&g.Scope=="all");
        var unit=p.Grants.Any(g=>g.Permissions.Contains("logs.review")&&g.Scope=="unit");
        var self=p.Grants.Any(g=>g.Permissions.Contains("logs.review")&&g.Scope=="self");
        return db.EquipmentLoans.AsNoTracking().Include(r=>r.Asset)
            .Where(r=>r.Status=="borrowed"||r.Status=="return_pending"||r.Status=="completed")
            .Where(r=>(p.Scope=="all"||p.Scope=="unit"&&r.DepartmentId==p.DepartmentId||p.Scope=="units"&&deps.Contains(r.DepartmentId)||p.Scope=="self"&&r.OwnerId==p.UserId)
                &&(all||unit&&r.DepartmentId==p.DepartmentId||self&&r.OwnerId==p.UserId));
    }
    public IQueryable<EquipmentLoan> Visible(AccessProfile p, bool review)
    {
        // Apply both account and granting-role scopes in SQL before count/pagination.
        var deps = p.DepartmentIds.ToArray();
        bool Has(string permission) => AccessRules.HasPermission(p, permission);
        bool Grant(string permission, string scope) => p.Grants.Any(g => g.Scope == scope && g.Permissions.Contains(permission));
        var borrow = Has("devices.borrow"); var reviewAllowed = Has("requests.review"); var manage = Has("resources.manage");
        var ra = Grant("requests.review", "all"); var ru = Grant("requests.review", "unit"); var rs = Grant("requests.review", "self");
        var ma = Grant("resources.manage", "all"); var mu = Grant("resources.manage", "unit");
        var ba = Grant("devices.borrow", "all"); var bu = Grant("devices.borrow", "unit"); var bs = Grant("devices.borrow", "self");
        return db.EquipmentLoans.AsNoTracking().Include(x => x.Asset).Where(r => review
            ? r.Status != "draft" && (reviewAllowed &&
                (p.Scope == "all" || p.Scope == "unit" && r.DepartmentId == p.DepartmentId || p.Scope == "units" && deps.Contains(r.DepartmentId) || p.Scope == "self" && r.OwnerId == p.UserId) &&
                (ra || ru && r.DepartmentId == p.DepartmentId || rs && r.OwnerId == p.UserId)
                || manage && (p.Scope == "all" || p.Scope == "unit" && r.Asset.DepartmentId == p.DepartmentId || p.Scope == "units" && deps.Contains(r.Asset.DepartmentId)) &&
                (ma || mu && r.Asset.DepartmentId == p.DepartmentId))
            : borrow && r.OwnerId == p.UserId &&
                (p.Scope == "all" || p.Scope == "self" || p.Scope == "unit" && r.DepartmentId == p.DepartmentId || p.Scope == "units" && deps.Contains(r.DepartmentId)) &&
                (ba || bs || bu && r.DepartmentId == p.DepartmentId));
    }
    public Task<EquipmentLoanView> Act(string actor, string id, string operation, string key, EquipmentCommand input, CancellationToken ct)
    {
        var ownerAction = operation is "submit" or "cancel" or "receipt";
        return commands.Run(actor, key, "equipment-loan." + operation + ":" + id, input,
            async () => { var r = await Get(actor, id, !ownerAction, ct); await AuthorizeAction(actor, r, operation, ct); }, async () => {
                var snapshot = await Get(actor, id, !ownerAction, ct);
                await TransactionLocks.Acquire(db, "asset:" + snapshot.AssetGroupId, ct);
                var r = await db.EquipmentLoans.SingleAsync(x => x.Id == id, ct);
                var a = await Asset(r.AssetGroupId, ct);
                await AuthorizeAction(actor, r, operation, ct); AdminService.Version(r.RowVersion, input.Version);
                var before = r.Status; var note = Note(input.Note);
                switch (operation)
                {
                    case "submit":
                        State(r, "draft"); await ValidateLoan(r, a, true, ct); r.Status = "pending"; break;
                    case "approve":
                        State(r, "pending"); await ValidateLoan(r, a, false, ct);
                        if (r.End <= DateTimeOffset.UtcNow) throw AppException.Invalid("Lịch mượn đã kết thúc.");
                        r.Status = "approved"; break;
                    case "reject": State(r, "pending"); r.Status = "rejected"; break;
                    case "cancel":
                        if (r.Status is not ("draft" or "pending")) throw AppException.Invalid("Chỉ hủy bản nháp hoặc phiếu chờ duyệt.");
                        r.Status = "cancelled"; break;
                    case "handover":
                        State(r, "approved");
                        if (input.Quantity != r.Quantity || r.End <= DateTimeOffset.UtcNow) throw AppException.Invalid("Số bàn giao phải khớp số đã duyệt và lịch còn hiệu lực.");
                        await ValidateLoan(r, a, false, ct);
                        var now = DateTimeOffset.UtcNow;
                        if (await Free(a, now, r.End, r.Id, ct) < r.Quantity) throw new AppException(409, "INSUFFICIENT_EQUIPMENT", "Thiết bị thực tế chưa đủ để bàn giao.");
                        r.HandedAt = now; r.Status = "ready";
                        Movement(actor, a.Id, r.Id, "handed.over", r.Quantity, note); break;
                    case "receipt":
                        State(r, "ready");
                        if (input.Quantity == null || input.Quantity < 0 || input.Quantity > r.Quantity) throw AppException.Invalid("Số thực nhận không hợp lệ.");
                        r.ReportedReceived = input.Quantity;
                        r.Status = input.Quantity == r.Quantity ? "borrowed" : "receipt_disputed"; break;
                    case "resolve-receipt":
                        State(r, "receipt_disputed"); r.Status = "ready"; break; // owner must confirm again; never fabricate receipt
                    default: throw AppException.Invalid("Thao tác không hợp lệ.");
                }
                r.Note = note;
                admin.Audit(actor, "equipment-loan." + operation, id, new { before, after = r.Status, input.Quantity, note });
                await db.SaveChangesAsync(ct); return await View(r, a, ct);
            }, ct);
    }
    private async Task AuthorizeAction(string actor, EquipmentLoan r, string operation, CancellationToken ct)
    {
        if (operation is "handover" or "resolve-receipt")
            await Demand(actor, "resources.manage", (await Asset(r.AssetGroupId, ct)).DepartmentId, null, ct);
        else if (operation is "approve" or "reject") await Demand(actor, "requests.review", r.DepartmentId, r.OwnerId, ct);
        else if (r.OwnerId != actor) throw AppException.Missing();
        else await Demand(actor, "devices.borrow", r.DepartmentId, r.OwnerId, ct);
    }
    private static void State(EquipmentLoan r, string expected)
    {
        if (r.Status != expected) throw AppException.Invalid("Trạng thái phiếu không cho phép thao tác này.");
    }
    private static int ReturnQuantity(int good, int damaged, int lost)
    {
        if (good is < 0 or > 1000000 || damaged is < 0 or > 1000000 || lost is < 0 or > 1000000 || (long)good + damaged + lost is < 1 or > 1000000)
            throw AppException.Invalid("Số lượng tốt, hỏng, mất phải không âm và tổng từ 1 đến 1.000.000.");
        return good + damaged + lost;
    }
    public Task<EquipmentLoanView> RequestReturn(string actor, string id, string key, ReturnEquipment input, CancellationToken ct) =>
        commands.Run(actor, key, "equipment-return.request:" + id, input, async () => { await Get(actor, id, false, ct); }, async () => {
            var snapshot = await Get(actor, id, false, ct);
            await TransactionLocks.Acquire(db, "asset:" + snapshot.AssetGroupId, ct);
            var r = await db.EquipmentLoans.SingleAsync(x => x.Id == id, ct);
            State(r, "borrowed"); AdminService.Version(r.RowVersion, input.Version);
            var quantity = ReturnQuantity(input.Good, input.Damaged, input.Lost);
            if (quantity > r.Quantity - r.Returned) throw AppException.Invalid("Số trả vượt phần còn phải xử lý.");
            db.EquipmentReturns.Add(new() { LoanId = id, Good = input.Good, Damaged = input.Damaged, Lost = input.Lost, Note = Note(input.Note) });
            r.Status = "return_pending";
            admin.Audit(actor, "equipment-return.request", id, input);
            await db.SaveChangesAsync(ct); return await View(r, await Asset(r.AssetGroupId, ct), ct);
        }, ct);
    public Task<EquipmentLoanView> AcceptReturn(string actor, string id, string key, AcceptEquipmentReturn input, CancellationToken ct) =>
        commands.Run(actor, key, "equipment-return.accept:" + id, input, async () => {
            var r = await Get(actor, id, true, ct); await Demand(actor, "resources.manage", r.Asset.DepartmentId, null, ct);
        }, async () => {
            var snapshot = await Get(actor, id, true, ct);
            await TransactionLocks.Acquire(db, "asset:" + snapshot.AssetGroupId, ct);
            var r = await db.EquipmentLoans.SingleAsync(x => x.Id == id, ct);
            var a = await db.AssetGroups.SingleAsync(x => x.Id == r.AssetGroupId, ct);
            await Demand(actor, "resources.manage", a.DepartmentId, null, ct);
            State(r, "return_pending"); AdminService.Version(r.RowVersion, input.Version);
            var pending = await db.EquipmentReturns.SingleOrDefaultAsync(x => x.Id == input.ReturnId && x.LoanId == id && x.AcceptedAt == null, ct) ?? throw AppException.Missing();
            var quantity = ReturnQuantity(input.Good, input.Damaged, input.Lost);
            if (quantity > r.Quantity - r.Returned || quantity > pending.Good + pending.Damaged + pending.Lost)
                throw AppException.Invalid("Tiếp nhận vượt phần đang yêu cầu trả hoặc còn phải xử lý.");
            pending.AcceptedGood = input.Good; pending.AcceptedDamaged = input.Damaged; pending.AcceptedLost = input.Lost;
            pending.AcceptedAt = DateTimeOffset.UtcNow; pending.AcceptedBy = actor; pending.AcceptanceNote = Note(input.Note);
            r.Returned += quantity; r.Status = r.Returned == r.Quantity ? "completed" : "borrowed";
            a.Damaged += input.Damaged; a.Lost += input.Lost;
            // Force parent version to advance even when only good units were returned.
            db.Entry(a).Property(x => x.Total).IsModified = true;
            if (input.Good > 0) Movement(actor, a.Id, id, "returned.good", input.Good, input.Note);
            if (input.Damaged > 0) Movement(actor, a.Id, id, "returned.damaged", input.Damaged, input.Note);
            if (input.Lost > 0) Movement(actor, a.Id, id, "returned.lost", input.Lost, input.Note);
            admin.Audit(actor, "equipment-return.accept", id, new { input, r.Returned, a.Damaged, a.Lost });
            await db.SaveChangesAsync(ct); return await View(r, a, ct);
        }, ct);
    public async Task<EquipmentLoanView> View(EquipmentLoan r, AssetGroup a, CancellationToken ct)
    {
        var returns = await Returns([r.Id], ct);
        return BuildView(r, a, returns.GetValueOrDefault(r.Id) ?? []);
    }
    public async Task<EquipmentLoanView[]> Views(IReadOnlyCollection<EquipmentLoan> rows, CancellationToken ct)
    {
        if (rows.Count == 0) return [];
        var returns = await Returns(rows.Select(r => r.Id).ToArray(), ct);
        return rows.Select(r => BuildView(r, r.Asset, returns.GetValueOrDefault(r.Id) ?? [])).ToArray();
    }
    private async Task<Dictionary<string, EquipmentReturnView[]>> Returns(string[] ids, CancellationToken ct)
    {
        var rows = await db.EquipmentReturns.AsNoTracking().Where(x => ids.Contains(x.LoanId))
            .OrderBy(x => x.At).ThenBy(x => x.Id)
            .Select(x => new { x.LoanId, View = new EquipmentReturnView(x.Id, x.Good, x.Damaged, x.Lost,
                x.AcceptedGood, x.AcceptedDamaged, x.AcceptedLost, x.Note, x.AcceptanceNote, x.At, x.AcceptedAt) }).ToArrayAsync(ct);
        return rows.GroupBy(x => x.LoanId).ToDictionary(g => g.Key, g => g.Select(x => x.View).ToArray());
    }
    private static EquipmentLoanView BuildView(EquipmentLoan r, AssetGroup a, EquipmentReturnView[] returns)
    {
        return new(r.Id, a.Id, a.Code, a.Name, r.OwnerId, r.DepartmentId, r.Start, r.End, r.Quantity, r.Returned,
            r.Purpose, r.Status, r.Note, r.ReportedReceived, Holds(r) && r.End < DateTimeOffset.UtcNow && r.Returned < r.Quantity,
            Convert.ToBase64String(r.RowVersion), returns);
    }
    private void Movement(string actor, string asset, string? loan, string kind, int quantity, string? note) =>
        db.EquipmentMovements.Add(new() { ActorId = actor, AssetGroupId = asset, LoanId = loan, Kind = kind, Quantity = quantity, Note = Note(note) });
}
