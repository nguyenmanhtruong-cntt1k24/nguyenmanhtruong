using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class RoomLogService(HpuniDbContext db, IAccessService access, AdminService admin)
{
    public static RoomLogView View(RoomUseLog l) => new(l.Id, l.RequestId, l.Used, l.ActualStart, l.ActualEnd,
        l.Attendees, l.Condition, l.Issue, l.Status, l.ReviewedBy, l.ReviewNote, Convert.ToBase64String(l.RowVersion));

    private async Task<RoomRequest> Demand(string actor, string requestId, bool review, CancellationToken ct)
    {
        var r = await db.RoomRequests.AsNoTracking().SingleOrDefaultAsync(x => x.Id == requestId && x.Status == "approved", ct)
            ?? throw AppException.Missing();
        var p = await access.GetProfile(actor, ct);
        if (!AccessRules.Allows(p, review ? "logs.review" : "logs.write", r.DepartmentId, r.OwnerId) || !review && r.OwnerId != actor)
            throw AppException.Missing();
        return r;
    }

    public async Task<RoomLogView> Get(string actor, string requestId, bool review, CancellationToken ct)
    {
        await Demand(actor, requestId, review, ct);
        return View(await db.RoomUseLogs.AsNoTracking().SingleOrDefaultAsync(x => x.RequestId == requestId, ct) ?? throw AppException.Missing());
    }

    public async Task<RoomLogView> Submit(string actor, string requestId, SaveRoomLog input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "room-log:" + requestId, ct);
        var request = await Demand(actor, requestId, false, ct);
        if (request.End > DateTimeOffset.UtcNow) throw AppException.Invalid("Chỉ ghi nhật ký sau khi kết thúc lịch đã duyệt.");
        var existing = await db.RoomUseLogs.SingleOrDefaultAsync(x => x.RequestId == requestId, ct);
        if (existing != null)
        {
            AdminService.Version(existing.RowVersion, input.Version);
            if (existing.Status != "needs_changes") throw AppException.Invalid("Nhật ký đã gửi; chỉ sửa khi được yêu cầu bổ sung.");
        }
        else if (input.Version != null) throw AppException.Stale();
        if (input.Used && (input.ActualStart == null || input.ActualEnd == null || input.Attendees == null ||
            input.Attendees is < 0 or > 10000 || input.ActualStart >= input.ActualEnd || input.ActualEnd > DateTimeOffset.UtcNow ||
            input.ActualEnd - input.ActualStart > TimeSpan.FromDays(1)))
            throw AppException.Invalid("Thời gian hoặc số người sử dụng thực tế không hợp lệ.");
        if (!input.Used && (input.ActualStart != null || input.ActualEnd != null || input.Attendees != null))
            throw AppException.Invalid("Không sử dụng phòng thì để trống thời gian và số người thực tế.");
        var log = existing ?? new RoomUseLog { RequestId = requestId };
        var before = existing == null ? null : View(existing);
        log.Used = input.Used; log.ActualStart = input.ActualStart?.ToUniversalTime(); log.ActualEnd = input.ActualEnd?.ToUniversalTime();
        log.Attendees = input.Attendees; log.Condition = Note(input.Condition); log.Issue = Note(input.Issue); log.Status = "submitted";
        if (existing == null) db.RoomUseLogs.Add(log);
        admin.Audit(actor, existing == null ? "room-log.submit" : "room-log.resubmit", log.Id,
            new { before, after = new { log.Used, log.ActualStart, log.ActualEnd, log.Attendees, log.Condition, log.Issue, log.Status } });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return View(log);
    }

    public async Task<RoomLogView> Review(string actor, string requestId, ReviewRoomLog input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "room-log:" + requestId, ct);
        await Demand(actor, requestId, true, ct);
        var log = await db.RoomUseLogs.SingleOrDefaultAsync(x => x.RequestId == requestId, ct) ?? throw AppException.Missing();
        AdminService.Version(log.RowVersion, input.Version);
        if (log.Status != "submitted" || input.Result is not ("confirmed" or "needs_changes")) throw AppException.Invalid("Kết quả hoặc trạng thái nhật ký không hợp lệ.");
        var before = View(log);
        log.Status = input.Result; log.ReviewedBy = actor; log.ReviewNote = Note(input.Note);
        admin.Audit(actor, "room-log.review", log.Id, new { before, result = input.Result, note = log.ReviewNote });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return View(log);
    }

    private static string Note(string? value)
    {
        value = value?.Trim() ?? "";
        if (value.Length > 2000) throw AppException.Invalid("Nội dung tối đa 2000 ký tự.");
        return value;
    }
}
