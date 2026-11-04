using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class AssetLifecycleService(HpuniDbContext db, IAccessService access, AdminService admin, EquipmentService equipment, IdempotentCommands commands)
{
    public static FixedAssetProfileView View(FixedAssetProfile p) => new(p.AssetGroupId, p.AcquisitionValue, p.AcquisitionDate, p.Source, p.Reference, p.Note, p.UpdatedAt, Convert.ToBase64String(p.RowVersion));
    public static AssetDisposalView View(AssetDisposal d) => new(d.Id, d.AssetGroupId, d.DepartmentId, d.Good, d.Damaged, d.Reason, d.Reference, d.Status,
        d.ActorId, d.ReviewerId, d.ReviewNote, d.CreatedAt, d.ReviewedAt, Convert.ToBase64String(d.RowVersion));
    static bool ReadAllowed(AccessProfile p, string department) => AccessRules.Allows(p, "resources.manage", department, null) || AccessRules.Allows(p, "budget.approve", department, null);
    async Task Demand(string actor, string permission, string department, CancellationToken ct)
    {
        if (!AccessRules.Allows(await access.GetProfile(actor, ct), permission, department, null)) throw AppException.Forbidden();
    }
    async Task<AssetGroup> Asset(string id, CancellationToken ct) => await db.AssetGroups.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
    public static IQueryable<AssetDisposal> Visible(IQueryable<AssetDisposal> query, AccessProfile p)
    {
        var manage = AccessRules.HasPermission(p, "resources.manage"); var review = AccessRules.HasPermission(p, "budget.approve");
        if (!manage && !review) throw AppException.Forbidden();
        var a = manage ? AssetOperationsService.Scoped(query, p, "resources.manage") : query.Where(x => false);
        var b = review ? AssetOperationsService.Scoped(query, p, "budget.approve") : query.Where(x => false);
        return a.Union(b);
    }
    public static IQueryable<AssetGroup> VisibleAssets(IQueryable<AssetGroup> query, AccessProfile p)
    {
        var manage = AccessRules.HasPermission(p, "resources.manage"); var review = AccessRules.HasPermission(p, "budget.approve");
        if (!manage && !review) throw AppException.Forbidden();
        var a = manage ? AssetOperationsService.Scoped(query, p, "resources.manage") : query.Where(x => false);
        var b = review ? AssetOperationsService.Scoped(query, p, "budget.approve") : query.Where(x => false);
        return a.Union(b);
    }
    public async Task<FixedAssetProfileView?> Profile(string actor, string assetId, CancellationToken ct)
    {
        var a = await Asset(assetId, ct); if (!ReadAllowed(await access.GetProfile(actor, ct), a.DepartmentId)) throw AppException.Forbidden();
        var p = await db.Set<FixedAssetProfile>().AsNoTracking().SingleOrDefaultAsync(x => x.AssetGroupId == assetId, ct); return p == null ? null : View(p);
    }
    public Task<FixedAssetProfileView> SaveProfile(string actor, string assetId, string key, SaveFixedAssetProfile input, CancellationToken ct) =>
        commands.Run(actor, key, "asset-profile.save:" + assetId, input,
            async () => { var a = await Asset(assetId, ct); await Demand(actor, "resources.manage", a.DepartmentId, ct); }, async () => {
                var snapshot = await Asset(assetId, ct); await TransactionLocks.Acquire(db, "asset:" + snapshot.Id, ct);
                var a = await Asset(snapshot.Id, ct); await Demand(actor, "resources.manage", a.DepartmentId, ct);
                var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
                if (input.AcquisitionValue < 1 || input.AcquisitionValue > 1000000000000m || decimal.Truncate(input.AcquisitionValue) != input.AcquisitionValue || input.AcquisitionDate < new DateOnly(1900, 1, 1) || input.AcquisitionDate > today)
                    throw AppException.Invalid("Nguyên giá phải là số nguyên VND từ 1 đến 1.000 tỷ; ngày mua từ 1900 đến hôm nay (giờ Việt Nam).");
                var p = await db.Set<FixedAssetProfile>().SingleOrDefaultAsync(x => x.AssetGroupId == assetId, ct);
                var before = p == null ? null : View(p);
                if (p == null)
                {
                    if (!string.IsNullOrEmpty(input.Version)) throw new AppException(409, "STALE_VERSION", "Hồ sơ nguyên giá đã thay đổi.");
                    p = new() { AssetGroupId = a.Id }; db.Add(p);
                }
                else AdminService.Version(p.RowVersion, input.Version ?? "");
                p.AcquisitionValue = input.AcquisitionValue; p.AcquisitionDate = input.AcquisitionDate; p.Source = AdminService.Required(input.Source, 200);
                p.Reference = AdminService.Required(input.Reference, 150); p.Note = AdminService.OptionalNote(input.Note); p.ActorId = actor; p.UpdatedAt = DateTimeOffset.UtcNow;
                admin.Audit(actor, "asset-profile.save", assetId, new { before, input }); await db.SaveChangesAsync(ct); return View(p);
            }, ct);
    static void Quantities(int good, int damaged)
    {
        if (good is < 0 or > 1000000 || damaged is < 0 or > 1000000 || good + damaged is < 1 or > 1000000) throw AppException.Invalid("Số thanh lý tốt/hỏng phải không âm, tổng từ 1 đến 1.000.000. Không thanh lý tài sản mất.");
    }
    async Task Capacity(AssetGroup a, int good, int damaged, CancellationToken ct)
    {
        // Disposal permanently shrinks capacity: protect all existing future reservations, not only free stock now.
        if (good > 0 && good > await equipment.Free(a, DateTimeOffset.UtcNow, DateTimeOffset.MaxValue, null, ct))
            throw new AppException(409, "DISPOSAL_RESERVED", "Phần tốt còn đang mượn, giữ lịch hoặc bảo dưỡng; không đủ để thanh lý vĩnh viễn.");
        var repair = await db.MaintenanceOrders.AsNoTracking().Where(x => x.AssetGroupId == a.Id && x.Mode == "repair" && (x.Status == "planned" || x.Status == "running")).SumAsync(x => (int?)x.Quantity, ct) ?? 0;
        if (damaged > 0 && damaged > a.Damaged - repair) throw new AppException(409, "DISPOSAL_DAMAGED_RESERVED", "Phần hỏng không đủ hoặc đang có kế hoạch sửa chữa.");
    }
    public Task<AssetDisposalView> Create(string actor, string key, CreateAssetDisposal input, CancellationToken ct) =>
        commands.Run(actor, key, "asset-disposal.create", input,
            async () => { var a = await Asset(input.AssetGroupId, ct); await Demand(actor, "resources.manage", a.DepartmentId, ct); }, async () => {
                Quantities(input.Good, input.Damaged); var snapshot = await Asset(input.AssetGroupId, ct); await TransactionLocks.Acquire(db, "asset:" + snapshot.Id, ct);
                var a = await Asset(snapshot.Id, ct); await Demand(actor, "resources.manage", a.DepartmentId, ct);
                await Capacity(a, input.Good, input.Damaged, ct);
                var request = new AssetDisposal { AssetGroupId = a.Id, DepartmentId = a.DepartmentId, Good = input.Good, Damaged = input.Damaged,
                    Reason = AdminService.Required(input.Reason, 2000), Reference = AdminService.Required(input.Reference, 150), ActorId = actor };
                db.Add(request); admin.Audit(actor, "asset-disposal.create", request.Id, input); await db.SaveChangesAsync(ct); return View(request);
            }, ct);
    public async Task<AssetDisposalView> Get(string actor, string id, CancellationToken ct)
    {
        var request = await db.Set<AssetDisposal>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        if (!ReadAllowed(await access.GetProfile(actor, ct), request.DepartmentId)) throw AppException.Forbidden(); return View(request);
    }
    public Task<AssetDisposalView> Review(string actor, string id, string operation, string key, ReviewAssetDisposal input, CancellationToken ct) =>
        commands.Run(actor, key, "asset-disposal." + operation + ":" + id, input,
            async () => { var d = await db.Set<AssetDisposal>().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing(); await Demand(actor, "budget.approve", d.DepartmentId, ct); }, async () => {
                var snapshot = await db.Set<AssetDisposal>().AsNoTracking().SingleAsync(x => x.Id == id, ct);
                await TransactionLocks.Acquire(db, "asset:" + snapshot.AssetGroupId, ct);
                var request = await db.Set<AssetDisposal>().SingleAsync(x => x.Id == id, ct); await Demand(actor, "budget.approve", request.DepartmentId, ct);
                AdminService.Version(request.RowVersion, input.Version);
                if (operation is not ("approve" or "reject") || request.Status != "pending") throw AppException.Invalid("Phiếu thanh lý không còn chờ duyệt.");
                if (request.ActorId == actor) throw AppException.Invalid("Người lập không được tự duyệt hoặc từ chối phiếu thanh lý.");
                var note = AdminService.Required(input.Note, 2000);
                if (operation == "approve")
                {
                    var a = await db.AssetGroups.SingleAsync(x => x.Id == request.AssetGroupId, ct);
                    if (a.DepartmentId != request.DepartmentId) throw new AppException(409, "ASSET_DEPARTMENT_CHANGED", "Đơn vị sở hữu đã thay đổi.");
                    await Capacity(a, request.Good, request.Damaged, ct);
                    a.Total -= request.Good + request.Damaged; a.Damaged -= request.Damaged;
                    if (request.Good > 0) db.EquipmentMovements.Add(new() { AssetGroupId = a.Id, ActorId = actor, Kind = "disposal.good", Quantity = -request.Good, Note = request.Id });
                    if (request.Damaged > 0) db.EquipmentMovements.Add(new() { AssetGroupId = a.Id, ActorId = actor, Kind = "disposal.damaged", Quantity = -request.Damaged, Note = request.Id });
                }
                request.Status = operation == "approve" ? "approved" : "rejected"; request.ReviewerId = actor; request.ReviewNote = note; request.ReviewedAt = DateTimeOffset.UtcNow;
                admin.Audit(actor, "asset-disposal." + operation, id, new { request.AssetGroupId, request.Good, request.Damaged, input }); await db.SaveChangesAsync(ct); return View(request);
            }, ct);
}
