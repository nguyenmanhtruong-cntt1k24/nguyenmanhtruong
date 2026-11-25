using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hpuni.Infrastructure;

public class RoomOptions
{
    public int OpenHour { get; set; } = 7;
    public int CloseHour { get; set; } = 18;
}

public class RoomService(HpuniDbContext db, IAccessService access, AdminService admin, IOptions<RoomOptions> options)
{
    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    public static RoomView View(Room r) => new(r.Id, r.Code, r.Name, r.Type, r.DepartmentId, r.Capacity, r.Active, Convert.ToBase64String(r.RowVersion));
    public static RoomRequestView View(RoomRequest r) => new(r.Id, r.RoomId, r.OwnerId, r.DepartmentId, r.Start, r.End,
        r.Attendees, r.Purpose, r.Status, r.ReviewedBy, r.ReviewNote, Convert.ToBase64String(r.RowVersion));
    private static RoomBlockView View(RoomReservation r) => new(r.Id, r.RoomId, r.Start, r.End, r.Active, Convert.ToBase64String(r.RowVersion));

    public async Task DemandDirectory(string actor, CancellationToken ct)
    {
        var p = await access.GetProfile(actor, ct);
        if (!new[] { "rooms.book", "resources.manage", "requests.review" }.Any(x => AccessRules.HasPermission(p, x)))
            throw AppException.Forbidden();
    }

    public async Task<RoomView> SaveRoom(string actor, string? id, SaveRoom input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var roomId = id ?? Guid.NewGuid().ToString();
        await TransactionLocks.Acquire(db, "room:" + roomId, ct);
        var p = await access.GetProfile(actor, ct);
        var room = id == null ? new Room { Id = roomId } : await db.Rooms.SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw AppException.Missing();
        if (!AccessRules.Allows(p, "resources.manage", id == null ? input.DepartmentId : room.DepartmentId, null)) throw AppException.Forbidden();
        if (!AccessRules.Allows(p, "resources.manage", input.DepartmentId, null)) throw AppException.Forbidden();
        if (id != null) AdminService.Version(room.RowVersion, input.Version);
        var before = id == null ? null : View(room);
        if (!await db.Departments.AnyAsync(x => x.Id == input.DepartmentId && x.Active, ct)) throw AppException.Invalid("Đơn vị không hợp lệ.");
        if (input.Capacity is < 1 or > 10000) throw AppException.Invalid("Sức chứa cần từ 1 đến 10000.");
        if (id != null)
        {
            var upcoming = db.RoomRequests.Where(x => x.RoomId == id && (x.Status == "pending" || x.Status == "approved") && x.End > DateTimeOffset.UtcNow);
            if (!input.Active && (await upcoming.AnyAsync(ct) || await db.RoomReservations.AnyAsync(x => x.RoomId == id && x.Active && x.End > DateTimeOffset.UtcNow, ct)))
                throw AppException.Invalid("Phòng còn lịch hiệu lực; chưa thể ngừng phục vụ.");
            if (await upcoming.AnyAsync(x => x.Attendees > input.Capacity, ct)) throw AppException.Invalid("Sức chứa mới nhỏ hơn sĩ số của lịch hiệu lực.");
        }
        room.Code = AdminService.Required(input.Code, 30).ToUpperInvariant();
        room.Name = AdminService.Required(input.Name, 150);
        room.Type = AdminService.Required(input.Type, 100);
        room.DepartmentId = input.DepartmentId; room.Capacity = input.Capacity; room.Active = input.Active;
        if (id == null) db.Rooms.Add(room);
        admin.Audit(actor, "room.save", room.Id, new { before, after = new { room.Code, room.Name, room.DepartmentId, room.Capacity, room.Active } });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return View(room);
    }

    public (DateTimeOffset Start, DateTimeOffset End) Day(DateOnly date)
    {
        DateTimeOffset At(int hour) => new(TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(TimeOnly.MinValue).AddHours(hour), Zone), TimeSpan.Zero);
        return (At(options.Value.OpenHour), At(options.Value.CloseHour));
    }

    public async Task<TimeSlot[]> Availability(string actor, string id, DateOnly date, CancellationToken ct)
    {
        await DemandDirectory(actor, ct);
        var room = await db.Rooms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        if (!room.Active) return [];
        var (start, end) = Day(date);
        var busy = await db.RoomReservations.AsNoTracking().Where(x => x.RoomId == id && x.Active && x.Start < end && x.End > start)
            .OrderBy(x => x.Start).Select(x => new TimeSlot(x.Start, x.End)).ToListAsync(ct);
        var free = new List<TimeSlot>();
        var cursor = start;
        foreach (var slot in busy)
        {
            if (slot.Start > cursor) free.Add(new(cursor, slot.Start < end ? slot.Start : end));
            if (slot.End > cursor) cursor = slot.End;
        }
        if (cursor < end) free.Add(new(cursor, end));
        return free.ToArray();
    }

    public async Task<RoomRequestView> Create(string actor, string key, SaveRoomRequest input, CancellationToken ct) =>
        await Once(actor, key, "room-request.create", input,
            async () => { if (!AccessRules.HasPermission(await access.GetProfile(actor, ct), "rooms.book")) throw AppException.Forbidden(); },
            async () =>
            {
                await TransactionLocks.Acquire(db, "room:" + input.RoomId, ct);
                var p = await access.GetProfile(actor, ct);
                if (!AccessRules.Allows(p, "rooms.book", p.DepartmentId, actor)) throw AppException.Forbidden();
                var room = await GetActiveRoom(input.RoomId, ct);
                Validate(input, room);
                var r = new RoomRequest { RoomId = room.Id, OwnerId = actor, DepartmentId = p.DepartmentId,
                    Start = input.Start.ToUniversalTime(), End = input.End.ToUniversalTime(), Attendees = input.Attendees, Purpose = Note(input.Purpose) };
                db.RoomRequests.Add(r);
                admin.Audit(actor, "room-request.create", r.Id, new { r.RoomId, r.Start, r.End, r.Attendees });
                await db.SaveChangesAsync(ct);
                return View(r);
            }, ct);

    public async Task<RoomRequestView> UpdateDraft(string actor, string id, SaveRoomRequest input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var known = await RequestAccess(actor, id, false, ct);
        await TransactionLocks.Acquire(db, "room:" + known.RoomId, ct);
        var r = await db.RoomRequests.SingleAsync(x => x.Id == id, ct);
        await RequestAccess(actor, id, false, ct);
        AdminService.Version(r.RowVersion, input.Version);
        if (r.Status != RoomRequestStatuses.Draft || input.RoomId != r.RoomId) throw AppException.Invalid("Chỉ được sửa nháp, không đổi phòng trên phiếu đã tạo.");
        Validate(input, await GetActiveRoom(r.RoomId, ct));
        r.Start = input.Start.ToUniversalTime(); r.End = input.End.ToUniversalTime(); r.Attendees = input.Attendees; r.Purpose = Note(input.Purpose);
        admin.Audit(actor, "room-request.update", id, new { r.Start, r.End, r.Attendees });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return View(r);
    }

    public async Task<RoomRequestView> Act(string actor, string id, string action, string key, RoomAction input, CancellationToken ct)
    {
        var review = action is "approve" or "reject";
        return await Once(actor, key, "room-request." + action + ":" + id, input,
            async () => { await RequestAccess(actor, id, review, ct); }, async () =>
            {
                var known = await RequestAccess(actor, id, review, ct);
                await TransactionLocks.Acquire(db, "room:" + known.RoomId, ct);
                var r = await db.RoomRequests.SingleAsync(x => x.Id == id, ct);
                await RequestAccess(actor, id, review, ct);
                AdminService.Version(r.RowVersion, input.Version);
                switch (action)
                {
                    case "submit":
                        if (r.Status != RoomRequestStatuses.Draft) throw AppException.Invalid("Chỉ gửi được phiếu nháp.");
                        Validate(new(r.RoomId, r.Start, r.End, r.Attendees, r.Purpose, null), await GetActiveRoom(r.RoomId, ct));
                        await NoConflict(r.RoomId, r.Start, r.End, null, ct);
                        db.RoomReservations.Add(new() { RoomId = r.RoomId, RequestId = r.Id, Start = r.Start, End = r.End });
                        r.Status = RoomRequestStatuses.Pending;
                        break;
                    case "approve":
                        if (r.Status != RoomRequestStatuses.Pending) throw AppException.Invalid("Phiếu không còn chờ duyệt.");
                        Validate(new(r.RoomId, r.Start, r.End, r.Attendees, r.Purpose, null), await GetActiveRoom(r.RoomId, ct));
                        await NoConflict(r.RoomId, r.Start, r.End, r.Id, ct);
                        if (!await db.RoomReservations.AnyAsync(x => x.RequestId == id && x.Active, ct)) throw AppException.Invalid("Phiếu không có giữ chỗ hiệu lực.");
                        r.Status = RoomRequestStatuses.Approved; r.ReviewedBy = actor; r.ReviewNote = Note(input.Note);
                        break;
                    case "reject":
                        if (r.Status != RoomRequestStatuses.Pending) throw AppException.Invalid("Phiếu không còn chờ duyệt.");
                        await Release(id, ct);
                        r.Status = RoomRequestStatuses.Rejected; r.ReviewedBy = actor; r.ReviewNote = Note(input.Note);
                        break;
                    case "cancel":
                        if (r.Status is not (RoomRequestStatuses.Draft or RoomRequestStatuses.Pending))
                            throw AppException.Invalid("Chỉ hủy được phiếu nháp hoặc đang chờ duyệt.");
                        await Release(id, ct);
                        r.Status = RoomRequestStatuses.Cancelled;
                        break;
                    default: throw AppException.Invalid("Thao tác không hợp lệ.");
                }
                admin.Audit(actor, "room-request." + action, r.Id, new { r.Status, r.ReviewNote });
                await db.SaveChangesAsync(ct);
                return View(r);
            }, ct);
    }

    public async Task<RoomRequest> RequestAccess(string actor, string id, bool review, CancellationToken ct)
    {
        var p = await access.GetProfile(actor, ct);
        var r = await db.RoomRequests.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        if (review && r.Status == RoomRequestStatuses.Draft) throw AppException.Missing();
        if (!AccessRules.Allows(p, review ? "requests.review" : "rooms.book", r.DepartmentId, r.OwnerId) || !review && r.OwnerId != actor)
            throw AppException.Missing();
        return r;
    }

    public async Task<RoomBlockView> Block(string actor, string roomId, string key, SaveRoomBlock input, CancellationToken ct) =>
        await Once(actor, key, "room-block.create:" + roomId, input,
            () => DemandManageRoom(actor, roomId, ct), async () =>
            {
                await TransactionLocks.Acquire(db, "room:" + roomId, ct);
                await DemandManageRoom(actor, roomId, ct);
                await GetActiveRoom(roomId, ct);
                if (input.Start >= input.End || input.Start < DateTimeOffset.UtcNow || input.End - input.Start > TimeSpan.FromDays(365))
                    throw AppException.Invalid("Thời gian chặn phòng không hợp lệ.");
                await NoConflict(roomId, input.Start, input.End, null, ct);
                var r = new RoomReservation { RoomId = roomId, Kind = "maintenance", Start = input.Start.ToUniversalTime(), End = input.End.ToUniversalTime(), Note = Note(input.Note) };
                db.RoomReservations.Add(r);
                admin.Audit(actor, "room-block.create", r.Id, new { r.RoomId, r.Start, r.End });
                await db.SaveChangesAsync(ct);
                return View(r);
            }, ct);

    public async Task<RoomBlockView> CancelBlock(string actor, string id, string key, RoomAction input, CancellationToken ct)
    {
        async Task Demand()
        {
            var item = await db.RoomReservations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.Kind == "maintenance", ct)
                ?? throw AppException.Missing();
            await DemandManageRoom(actor, item.RoomId, ct);
        }
        return await Once(actor, key, "room-block.cancel:" + id, input, Demand, async () =>
        {
            var known = await db.RoomReservations.AsNoTracking().SingleAsync(x => x.Id == id, ct);
            await TransactionLocks.Acquire(db, "room:" + known.RoomId, ct);
            await Demand();
            var item = await db.RoomReservations.SingleAsync(x => x.Id == id, ct);
            AdminService.Version(item.RowVersion, input.Version);
            if (!item.Active) throw AppException.Stale();
            item.Active = false;
            admin.Audit(actor, "room-block.cancel", id, new { });
            await db.SaveChangesAsync(ct);
            return View(item);
        }, ct);
    }

    public async Task DemandManageRoom(string actor, string id, CancellationToken ct)
    {
        var room = await db.Rooms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        if (!AccessRules.Allows(await access.GetProfile(actor, ct), "resources.manage", room.DepartmentId, null)) throw AppException.Missing();
    }

    private async Task<Room> GetActiveRoom(string id, CancellationToken ct) =>
        await db.Rooms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.Active, ct) ?? throw AppException.Invalid("Phòng không tồn tại hoặc đã ngừng phục vụ.");

    private async Task NoConflict(string roomId, DateTimeOffset start, DateTimeOffset end, string? except, CancellationToken ct)
    {
        if (await db.RoomReservations.AnyAsync(x => x.RoomId == roomId && x.Active && x.Start < end && x.End > start && (except == null || x.RequestId == null || x.RequestId != except), ct))
            throw new AppException(409, "ROOM_TIME_CONFLICT", "Khoảng giờ đã được giữ chỗ hoặc đang bảo trì. Vui lòng chọn lại.");
    }

    private async Task Release(string requestId, CancellationToken ct)
    {
        var items = await db.RoomReservations.Where(x => x.RequestId == requestId && x.Active).ToListAsync(ct);
        foreach (var item in items) item.Active = false;
    }

    private void Validate(SaveRoomRequest input, Room room)
    {
        var date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(input.Start, Zone).DateTime);
        var (start, end) = Day(date);
        if (input.Start >= input.End || input.Start < start || input.End > end || input.Start < DateTimeOffset.UtcNow)
            throw AppException.Invalid("Khoảng giờ cần ở tương lai, cùng ngày và trong giờ phục vụ.");
        if (input.Attendees < 1 || input.Attendees > room.Capacity) throw AppException.Invalid("Sĩ số vượt sức chứa hoặc không hợp lệ.");
        Note(input.Purpose);
    }

    private static string Note(string? value)
    {
        value = value?.Trim() ?? "";
        if (value.Length > 2000) throw AppException.Invalid("Nội dung tối đa 2000 ký tự.");
        return value;
    }

    private async Task<T> Once<T>(string actor, string key, string operation, object payload,
        Func<Task> authorize, Func<Task<T>> action, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 100 || key.Any(char.IsControl))
            throw AppException.Invalid("Cần Idempotency-Key từ 1 đến 100 ký tự.");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload))));
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var lockKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(actor + ":" + key)));
        await TransactionLocks.Acquire(db, "idempotency:" + lockKey, ct);
        await authorize();
        var existing = await db.IdempotencyRecords.SingleOrDefaultAsync(x => x.ActorId == actor && x.Key == key, ct);
        if (existing != null)
        {
            if (existing.Operation != operation || existing.PayloadHash != hash)
                throw new AppException(409, "IDEMPOTENCY_CONFLICT", "Khóa gửi lặp đã dùng với nội dung khác.");
            var stored = JsonSerializer.Deserialize<T>(existing.ResultJson)!;
            await tx.CommitAsync(ct);
            return stored;
        }
        var result = await action();
        db.IdempotencyRecords.Add(new() { ActorId = actor, Key = key, Operation = operation, PayloadHash = hash, ResultJson = JsonSerializer.Serialize(result) });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result;
    }
}
