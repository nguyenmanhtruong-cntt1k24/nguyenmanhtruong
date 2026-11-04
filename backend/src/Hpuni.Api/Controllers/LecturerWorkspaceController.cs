using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1/lecturer-workspace"), Authorize]
public class LecturerWorkspaceController(HpuniDbContext db, IAccessService access, EquipmentService equipment) : ControllerBase
{
    string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    static void Page(int page, int pageSize, string? search = null)
    {
        if (page is < 1 or > 100000 || pageSize is < 1 or > 100 || search?.Length > 150)
            throw AppException.Invalid("Bộ lọc hoặc phân trang không hợp lệ.");
    }
    static bool Has(AccessProfile p, string permission) => AccessRules.HasPermission(p, permission);
    static DateOnly TodayVn() => DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime);
    static DateTimeOffset DayStart(DateOnly d) => new(d.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(7));
    static DateTimeOffset AtVn(DateOnly d, TimeOnly t) => new(d.ToDateTime(t), TimeSpan.FromHours(7));
    static string[] SplitIds(string? csv) => string.IsNullOrWhiteSpace(csv)
        ? [] : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct().Take(50).ToArray();

    static bool HasFreeWindow(DateTimeOffset open, DateTimeOffset close, List<(DateTimeOffset Start, DateTimeOffset End)>? busy)
    {
        if (busy is null || busy.Count == 0) return true;
        var cursor = open;
        foreach (var (rawStart, rawEnd) in busy.OrderBy(x => x.Start))
        {
            var s = rawStart < open ? open : rawStart;
            var e = rawEnd > close ? close : rawEnd;
            if (e <= s) continue;
            if (s > cursor) return true;
            if (e > cursor) cursor = e;
            if (cursor >= close) return false;
        }
        return cursor < close;
    }

    IQueryable<RoomRequest> MyRooms(AccessProfile p, string permission = "rooms.book")
    {
        if (!Has(p, permission)) return db.RoomRequests.AsNoTracking().Where(_ => false);
        var deps = p.DepartmentIds.ToArray();
        var grants = p.Grants.Where(g => g.Permissions.Contains(permission)).ToArray();
        var all = grants.Any(g => g.Scope == Scopes.All);
        var unit = grants.Any(g => g.Scope == Scopes.Unit);
        var units = grants.Any(g => g.Scope == Scopes.Units);
        var self = grants.Any(g => g.Scope == Scopes.Self);
        return db.RoomRequests.AsNoTracking().Where(r => r.OwnerId == p.UserId
            && (p.Scope == Scopes.All || p.Scope == Scopes.Unit && r.DepartmentId == p.DepartmentId || p.Scope == Scopes.Units && deps.Contains(r.DepartmentId) || p.Scope == Scopes.Self && r.OwnerId == p.UserId)
            && (all || unit && r.DepartmentId == p.DepartmentId || units && deps.Contains(r.DepartmentId) || self && r.OwnerId == p.UserId));
    }

    IQueryable<EquipmentLoan> MyLoans(AccessProfile p, string permission = "devices.borrow")
    {
        if (!Has(p, permission)) return db.EquipmentLoans.AsNoTracking().Where(_ => false);
        var source = Has(p, "devices.borrow") ? equipment.Visible(p, false).AsNoTracking() : db.EquipmentLoans.AsNoTracking();
        var deps = p.DepartmentIds.ToArray();
        var grants = p.Grants.Where(g => g.Permissions.Contains(permission)).ToArray();
        var all = grants.Any(g => g.Scope == Scopes.All);
        var unit = grants.Any(g => g.Scope == Scopes.Unit);
        var units = grants.Any(g => g.Scope == Scopes.Units);
        var self = grants.Any(g => g.Scope == Scopes.Self);
        return source.Where(r => r.OwnerId == p.UserId
            && (p.Scope == Scopes.All || p.Scope == Scopes.Unit && r.DepartmentId == p.DepartmentId || p.Scope == Scopes.Units && deps.Contains(r.DepartmentId) || p.Scope == Scopes.Self && r.OwnerId == p.UserId)
            && (all || unit && r.DepartmentId == p.DepartmentId || units && deps.Contains(r.DepartmentId) || self && r.OwnerId == p.UserId));
    }

    async Task<int> PendingCount(AccessProfile p, CancellationToken ct) =>
        await MyRooms(p, "rooms.book").CountAsync(r => r.Status == RoomRequestStatuses.Pending, ct) +
        await MyLoans(p, "devices.borrow").CountAsync(r => r.Status == "pending", ct);

    async Task<List<object>> Hydrate(IReadOnlyList<(string Kind, string Id)> refs, IQueryable<RoomRequest> rooms, IQueryable<EquipmentLoan> loans, CancellationToken ct)
    {
        if (refs.Count == 0) return [];
        var rIds = refs.Where(x => x.Kind == "room").Select(x => x.Id).Distinct().ToArray();
        var lIds = refs.Where(x => x.Kind == "device").Select(x => x.Id).Distinct().ToArray();
        var rRows = rIds.Length > 0 ? await rooms.Where(r => rIds.Contains(r.Id)).ToArrayAsync(ct) : [];
        var lRows = lIds.Length > 0 ? await loans.Where(r => lIds.Contains(r.Id)).Include(r => r.Asset).ToArrayAsync(ct) : [];
        var roomTargetIds = rRows.Select(r => r.RoomId).Distinct().ToArray();
        var names = roomTargetIds.Length > 0 ? await db.Rooms.AsNoTracking().Where(r => roomTargetIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.Name, ct) : [];
        var rLogs = rIds.Length > 0 ? await db.RoomUseLogs.AsNoTracking().Where(l => rIds.Contains(l.RequestId)).ToDictionaryAsync(l => l.RequestId, l => l.Status, ct) : [];
        var lLogs = lIds.Length > 0 ? await db.EquipmentUseLogs.AsNoTracking().Where(l => lIds.Contains(l.LoanId)).ToDictionaryAsync(l => l.LoanId, l => l.Status, ct) : [];
        var map = new Dictionary<string, object>(refs.Count);
        foreach (var r in rRows) map["room:" + r.Id] = new { kind = "room", request = (object)RoomService.View(r), name = names.GetValueOrDefault(r.RoomId, r.RoomId), logStatus = rLogs.GetValueOrDefault(r.Id) };
        var loanViews = (await equipment.Views(lRows, ct)).ToDictionary(v => v.Id);
        foreach (var r in lRows) map["device:" + r.Id] = new { kind = "device", request = (object)loanViews[r.Id], name = r.Asset.Name, manageDepartmentId = r.Asset.DepartmentId, logStatus = lLogs.GetValueOrDefault(r.Id) };
        return refs.Where(s => map.ContainsKey(s.Kind + ":" + s.Id)).Select(s => map[s.Kind + ":" + s.Id]).ToList();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] DateOnly? date = null, CancellationToken ct = default)
    {
        var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "rooms.book") && !Has(p, "devices.borrow") && !Has(p, "logs.write")) throw AppException.Forbidden();
        var today = TodayVn(); var todayStart = DayStart(today); var tomorrowStart = DayStart(today.AddDays(1));
        var selected = date ?? today; var weekStartDay = selected.AddDays(-(((int)selected.DayOfWeek + 6) % 7));
        var weekStart = DayStart(weekStartDay); var weekEnd = DayStart(weekStartDay.AddDays(7));
        var rooms = MyRooms(p, "rooms.book"); var loans = MyLoans(p, "devices.borrow");
        var todayApproved = await rooms.CountAsync(r => r.Status == RoomRequestStatuses.Approved && r.Start >= todayStart && r.Start < tomorrowStart, ct)
            + await loans.CountAsync(r => r.Status == "approved" && r.Start >= todayStart && r.Start < tomorrowStart, ct);
        var pendingCount = await PendingCount(p, ct);
        var borrowedDevices = await loans.Where(r => r.Status == "borrowed" || r.Status == "return_pending").SumAsync(r => (int?)(r.Quantity - r.Returned), ct) ?? 0;
        var logsNeeded = 0; var now = DateTimeOffset.UtcNow;
        if (Has(p, "logs.write"))
        {
            logsNeeded = await MyRooms(p, "logs.write").CountAsync(r => r.Status == RoomRequestStatuses.Approved && r.End <= now && !db.RoomUseLogs.Any(l => l.RequestId == r.Id), ct)
                + await MyLoans(p, "logs.write").CountAsync(r => (r.Status == "borrowed" || r.Status == "return_pending" || r.Status == "completed") && r.End <= now && !db.EquipmentUseLogs.Any(l => l.LoanId == r.Id), ct);
        }
        var agendaIds = await rooms.Where(r => r.Status == RoomRequestStatuses.Approved && r.Start >= weekStart && r.Start < weekEnd).OrderBy(r => r.Start).ThenBy(r => r.Id).Select(r => r.Id).ToArrayAsync(ct);
        var recentRefs = await rooms.Select(r => new { kind = "room", r.Id, at = r.CreatedAt }).Concat(loans.Select(r => new { kind = "device", r.Id, at = r.CreatedAt }))
            .OrderByDescending(r => r.at).ThenBy(r => r.Id).Take(4).ToArrayAsync(ct);
        var agenda = await Hydrate(agendaIds.Select(id => ("room", id)).ToArray(), rooms, loans, ct);
        var recent = await Hydrate(recentRefs.Select(r => (r.kind, r.Id)).ToArray(), rooms, loans, ct);
        return Ok(new { todayApproved, pendingCount, borrowedDevices, logsNeeded, selectedDate = selected, weekStart = weekStartDay, agenda, recent, generatedAt = DateTimeOffset.UtcNow });
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> Calendar([FromQuery] DateOnly? date = null, CancellationToken ct = default)
    {
        var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "rooms.book") && !Has(p, "devices.borrow")) throw AppException.Forbidden();
        var selected = date ?? TodayVn(); var weekStartDay = selected.AddDays(-(((int)selected.DayOfWeek + 6) % 7));
        var weekStart = DayStart(weekStartDay); var weekEnd = DayStart(weekStartDay.AddDays(7));
        var rooms = MyRooms(p, "rooms.book");
        var ids = await rooms.Where(r => r.Status == RoomRequestStatuses.Approved && r.Start >= weekStart && r.Start < weekEnd).OrderBy(r => r.Start).ThenBy(r => r.Id).Select(r => r.Id).ToArrayAsync(ct);
        var items = await Hydrate(ids.Select(id => ("room", id)).ToArray(), rooms, MyLoans(p, "devices.borrow"), ct);
        return Ok(new { selectedDate = selected, weekStart = weekStartDay, items, pendingCount = await PendingCount(p, ct) });
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> Rooms([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null,
        [FromQuery] string? type = null, [FromQuery] int? capacity = null, [FromQuery] DateOnly? date = null, [FromQuery] string? start = null,
        [FromQuery] string? end = null, [FromQuery] string? departmentIds = null, [FromQuery] string? roomId = null, CancellationToken ct = default)
    {
        Page(page, pageSize, search); var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "rooms.book") && !Has(p, "resources.manage") && !Has(p, "requests.review")) throw AppException.Forbidden();
        if (!string.IsNullOrEmpty(roomId))
        {
            var single = await db.Rooms.AsNoTracking().Where(r => r.Id == roomId && r.Active).ToArrayAsync(ct);
            return Ok(new { items = single.Select(RoomService.View), total = single.Length, page = 1, pageSize = 1, types = Array.Empty<string>(), pendingCount = await PendingCount(p, ct) });
        }
        var types = await db.Rooms.AsNoTracking().Where(r => r.Active && r.Type != "").Select(r => r.Type).Distinct().OrderBy(t => t).ToArrayAsync(ct);
        var q = db.Rooms.AsNoTracking().Where(r => r.Active);
        if (!string.IsNullOrWhiteSpace(type)) q = q.Where(r => r.Type == type);
        if (capacity is > 0) q = q.Where(r => r.Capacity >= capacity.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim(); var deptIds = SplitIds(departmentIds);
            q = deptIds.Length > 0 ? q.Where(r => r.Code.Contains(s) || r.Name.Contains(s) || r.Type.Contains(s) || deptIds.Contains(r.DepartmentId))
                : q.Where(r => r.Code.Contains(s) || r.Name.Contains(s) || r.Type.Contains(s));
        }
        var d = date ?? TodayVn(); var open = AtVn(d, new TimeOnly(7, 0)); var close = AtVn(d, new TimeOnly(18, 0));
        if (!string.IsNullOrWhiteSpace(start) || !string.IsNullOrWhiteSpace(end))
        {
            if (!TimeOnly.TryParse(start, out var st) || !TimeOnly.TryParse(end, out var et) || st < new TimeOnly(7, 0) || et > new TimeOnly(18, 0) || et <= st)
                throw AppException.Invalid("Khoảng giờ tìm phòng không hợp lệ.");
            var sAt = AtVn(d, st); var eAt = AtVn(d, et);
            q = q.Where(r => !db.RoomReservations.Any(b => b.RoomId == r.Id && b.Active && b.Start < eAt && b.End > sAt)
                && !db.RoomRequests.Any(x => x.RoomId == r.Id && (x.Status == RoomRequestStatuses.Pending || x.Status == RoomRequestStatuses.Approved) && x.Start < eAt && x.End > sAt));
            var total = await q.CountAsync(ct);
            var rows = await q.OrderBy(r => r.Code).ThenBy(r => r.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
            return Ok(new { items = rows.Select(RoomService.View), total, page, pageSize, types, pendingCount = await PendingCount(p, ct) });
        }
        q = q.Where(r => !db.RoomReservations.Any(b => b.RoomId == r.Id && b.Active && b.Start <= open && b.End >= close)
            && !db.RoomRequests.Any(x => x.RoomId == r.Id && (x.Status == RoomRequestStatuses.Pending || x.Status == RoomRequestStatuses.Approved) && x.Start <= open && x.End >= close));
        var candidateIds = await q.OrderBy(r => r.Code).ThenBy(r => r.Id).Select(r => r.Id).ToArrayAsync(ct);
        var resBusy = candidateIds.Length > 0 ? await db.RoomReservations.AsNoTracking().Where(b => candidateIds.Contains(b.RoomId) && b.Active && b.Start < close && b.End > open).Select(b => new { b.RoomId, b.Start, b.End }).ToArrayAsync(ct) : [];
        var reqBusy = candidateIds.Length > 0 ? await db.RoomRequests.AsNoTracking().Where(x => candidateIds.Contains(x.RoomId) && (x.Status == RoomRequestStatuses.Pending || x.Status == RoomRequestStatuses.Approved) && x.Start < close && x.End > open).Select(x => new { x.RoomId, x.Start, x.End }).ToArrayAsync(ct) : [];
        var byRoom = resBusy.Concat(reqBusy).GroupBy(x => x.RoomId).ToDictionary(g => g.Key, g => g.Select(v => (v.Start, v.End)).ToList());
        var freeIds = candidateIds.Where(id => HasFreeWindow(open, close, byRoom.GetValueOrDefault(id))).ToArray();
        var pageIds = freeIds.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
        var pageMap = pageIds.Length > 0 ? await db.Rooms.AsNoTracking().Where(r => pageIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, ct) : [];
        var pageRows = pageIds.Where(pageMap.ContainsKey).Select(id => RoomService.View(pageMap[id])).ToArray();
        return Ok(new { items = pageRows, total = freeIds.Length, page, pageSize, types, pendingCount = await PendingCount(p, ct) });
    }

    [HttpGet("devices")]
    public async Task<IActionResult> Devices([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null,
        [FromQuery] string? category = null, [FromQuery] string? departmentIds = null, [FromQuery] string? assetGroupId = null, CancellationToken ct = default)
    {
        Page(page, pageSize, search); var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "devices.borrow") && !Has(p, "resources.manage") && !Has(p, "requests.review")) throw AppException.Forbidden();
        if (!string.IsNullOrEmpty(assetGroupId))
        {
            var single = await db.AssetGroups.AsNoTracking().Where(a => a.Id == assetGroupId && a.Active).ToArrayAsync(ct);
            return Ok(new { items = await equipment.AssetViews(single, ct), total = single.Length, page = 1, pageSize = 1, categories = Array.Empty<string>(), pendingCount = await PendingCount(p, ct) });
        }
        var categories = await db.AssetGroups.AsNoTracking().Where(a => a.Active && a.Category != "").Select(a => a.Category).Distinct().OrderBy(c => c).ToArrayAsync(ct);
        var q = db.AssetGroups.AsNoTracking().Where(a => a.Active);
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(a => a.Category == category);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim(); var deptIds = SplitIds(departmentIds);
            q = deptIds.Length > 0 ? q.Where(a => a.Code.Contains(s) || a.Name.Contains(s) || a.Category.Contains(s) || deptIds.Contains(a.DepartmentId))
                : q.Where(a => a.Code.Contains(s) || a.Name.Contains(s) || a.Category.Contains(s));
        }
        var total = await q.CountAsync(ct);
        var rows = await q.OrderBy(a => a.Code).ThenBy(a => a.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new { items = await equipment.AssetViews(rows, ct), total, page, pageSize, categories, pendingCount = await PendingCount(p, ct) });
    }

    [HttpGet("requests")]
    public async Task<IActionResult> Requests([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? tab = "all",
        [FromQuery] string? search = null, [FromQuery] string? status = null, [FromQuery] string? id = null, CancellationToken ct = default)
    {
        Page(page, pageSize, search); var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "rooms.book") && !Has(p, "devices.borrow")) throw AppException.Forbidden();
        var baseRooms = MyRooms(p, "rooms.book"); var baseLoans = MyLoans(p, "devices.borrow");
        var navPending = await PendingCount(p, ct);
        if (!string.IsNullOrEmpty(id)) { baseRooms = baseRooms.Where(r => r.Id == id); baseLoans = baseLoans.Where(r => r.Id == id); }
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            baseRooms = baseRooms.Where(r => r.Id.Contains(s) || r.Purpose.Contains(s) || db.Rooms.Any(a => a.Id == r.RoomId && (a.Name.Contains(s) || a.Code.Contains(s))));
            baseLoans = baseLoans.Where(r => r.Id.Contains(s) || r.Purpose.Contains(s) || r.Asset.Name.Contains(s) || r.Asset.Code.Contains(s));
        }
        var roomCount = await baseRooms.CountAsync(ct); var deviceCount = await baseLoans.CountAsync(ct);
        var pendingCount = await baseRooms.CountAsync(r => r.Status == RoomRequestStatuses.Pending, ct) + await baseLoans.CountAsync(r => r.Status == "pending", ct);
        var draftCount = await baseRooms.CountAsync(r => r.Status == RoomRequestStatuses.Draft, ct) + await baseLoans.CountAsync(r => r.Status == "draft", ct);
        var rooms = baseRooms; var loans = baseLoans;
        if (tab == "room") loans = loans.Where(_ => false);
        else if (tab == "device") rooms = rooms.Where(_ => false);
        else if (tab is "pending" or "draft") { rooms = rooms.Where(r => r.Status == tab); loans = loans.Where(r => r.Status == tab); }
        if (!string.IsNullOrWhiteSpace(status)) { rooms = rooms.Where(r => r.Status == status); loans = loans.Where(r => r.Status == status); }
        var refs = rooms.Select(r => new { kind = "room", r.Id, at = r.CreatedAt }).Concat(loans.Select(r => new { kind = "device", r.Id, at = r.CreatedAt }));
        var total = await refs.CountAsync(ct);
        var selected = await refs.OrderByDescending(r => r.at).ThenBy(r => r.Id).ThenBy(r => r.kind).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        var items = await Hydrate(selected.Select(r => (r.kind, r.Id)).ToArray(), baseRooms, baseLoans, ct);
        return Ok(new { items, total, page, pageSize, counts = new { all = roomCount + deviceCount, room = roomCount, device = deviceCount, pending = pendingCount, draft = draftCount }, pendingCount = navPending });
    }

    [HttpGet("returns")]
    public async Task<IActionResult> Returns([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int readyPage = 1, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        Page(page, pageSize, search); Page(readyPage, pageSize, search); var p = await access.GetProfile(Actor, ct);
        if (!Has(p, "devices.borrow")) throw AppException.Forbidden();
        var loans = MyLoans(p, "devices.borrow");
        if (!string.IsNullOrWhiteSpace(search)) { var s = search.Trim(); loans = loans.Where(r => r.Id.Contains(s) || r.Purpose.Contains(s) || r.Asset.Name.Contains(s) || r.Asset.Code.Contains(s)); }
        var active = loans.Where(r => r.Status == "borrowed" || r.Status == "return_pending");
        var ready = loans.Where(r => r.Status == "ready");
        var total = await active.CountAsync(ct); var readyTotal = await ready.CountAsync(ct);
        var activeIds = await active.OrderByDescending(r => r.CreatedAt).ThenBy(r => r.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(r => r.Id).ToArrayAsync(ct);
        var readyIds = await ready.OrderByDescending(r => r.CreatedAt).ThenBy(r => r.Id).Skip((readyPage - 1) * pageSize).Take(pageSize).Select(r => r.Id).ToArrayAsync(ct);
        var rooms = MyRooms(p, "rooms.book");
        return Ok(new { items = await Hydrate(activeIds.Select(id => ("device", id)).ToArray(), rooms, loans, ct), total, page, pageSize, readyItems = await Hydrate(readyIds.Select(id => ("device", id)).ToArray(), rooms, loans, ct), readyTotal, readyPage, pendingCount = await PendingCount(p, ct) });
    }

    [HttpGet("logs")]
    public async Task<IActionResult> Logs([FromQuery] int page = 1, [FromQuery] int eligiblePage = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        Page(page, pageSize, search); Page(eligiblePage, pageSize, search);
        var p = await access.GetProfile(Actor, ct); if (!Has(p, "logs.write")) throw AppException.Forbidden();
        var now = DateTimeOffset.UtcNow;
        var rooms = MyRooms(p, "logs.write"); var loans = MyLoans(p, "logs.write");
        var eRooms = rooms.Where(r => r.Status == RoomRequestStatuses.Approved && r.End <= now && !db.RoomUseLogs.Any(l => l.RequestId == r.Id));
        var eLoans = loans.Where(r => (r.Status == "borrowed" || r.Status == "return_pending" || r.Status == "completed") && r.End <= now && !db.EquipmentUseLogs.Any(l => l.LoanId == r.Id));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            eRooms = eRooms.Where(r => r.Id.Contains(s) || r.Purpose.Contains(s) || db.Rooms.Any(a => a.Id == r.RoomId && a.Name.Contains(s)));
            eLoans = eLoans.Where(r => r.Id.Contains(s) || r.Purpose.Contains(s) || r.Asset.Name.Contains(s));
        }
        var eRefs = eRooms.Select(r => new { kind = "room", r.Id, at = r.Start }).Concat(eLoans.Select(r => new { kind = "device", r.Id, at = r.Start }));
        var eligibleTotal = await eRefs.CountAsync(ct);
        var eSelected = await eRefs.OrderByDescending(r => r.at).ThenBy(r => r.Id).Skip((eligiblePage - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        var eligibleItems = await Hydrate(eSelected.Select(r => (r.kind, r.Id)).ToArray(), rooms, loans, ct);
        var rq = from l in db.RoomUseLogs.AsNoTracking() join r in rooms on l.RequestId equals r.Id join a in db.Rooms on r.RoomId equals a.Id select new { kind = "room", l.Id, requestId = r.Id, at = r.Start, l.Status, name = a.Name };
        var eq = from l in db.EquipmentUseLogs.AsNoTracking() join r in loans on l.LoanId equals r.Id select new { kind = "device", l.Id, requestId = r.Id, at = r.Start, l.Status, name = r.Asset.Name };
        var lq = rq.Concat(eq);
        if (!string.IsNullOrWhiteSpace(search)) { var s = search.Trim(); lq = lq.Where(r => r.name.Contains(s) || r.Id.Contains(s) || r.requestId.Contains(s)); }
        var total = await lq.CountAsync(ct);
        var lSelected = await lq.OrderByDescending(r => r.at).ThenBy(r => r.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        var rIds = lSelected.Where(r => r.kind == "room").Select(r => r.requestId).ToArray();
        var dIds = lSelected.Where(r => r.kind == "device").Select(r => r.requestId).ToArray();
        var rMap = rIds.Length > 0 ? await rooms.Where(r => rIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, ct) : [];
        var dMap = dIds.Length > 0 ? await loans.Where(r => dIds.Contains(r.Id)).Include(r => r.Asset).ToDictionaryAsync(r => r.Id, ct) : [];
        var rLogs = rIds.Length > 0 ? await db.RoomUseLogs.AsNoTracking().Where(l => rIds.Contains(l.RequestId)).ToDictionaryAsync(l => l.RequestId, ct) : [];
        var dLogs = dIds.Length > 0 ? await db.EquipmentUseLogs.AsNoTracking().Where(l => dIds.Contains(l.LoanId)).ToDictionaryAsync(l => l.LoanId, ct) : [];
        var loanViews = (await equipment.Views(dMap.Values.ToArray(), ct)).ToDictionary(v => v.Id);
        var items = new List<object>(lSelected.Length);
        foreach (var r in lSelected)
        {
            if (r.kind == "room") items.Add(new { r.kind, r.name, request = (object)RoomService.View(rMap[r.requestId]), log = (object)RoomLogService.View(rLogs[r.requestId]) });
            else { var loan = dMap[r.requestId]; var l = dLogs[r.requestId]; items.Add(new { r.kind, r.name, manageDepartmentId = loan.Asset.DepartmentId, request = (object)loanViews[loan.Id], log = (object)new EquipmentLogView(l.Id, l.LoanId, l.Used, l.ActualStart, l.ActualEnd, l.Quantity, l.Condition, l.Issue, l.Status, l.ReviewNote, Convert.ToBase64String(l.RowVersion)) }); }
        }
        return Ok(new { items, total, page, pageSize, eligibleItems, eligibleTotal, eligiblePage, pendingCount = await PendingCount(p, ct) });
    }
}
