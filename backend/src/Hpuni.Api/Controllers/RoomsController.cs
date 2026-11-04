using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1"), Authorize]
public class RoomsController(HpuniDbContext db, RoomService service, IAccessService access) : ControllerBase
{
    private string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string Key => Request.Headers["Idempotency-Key"].ToString();

    [HttpGet("rooms")]
    public async Task<IActionResult> Rooms([FromQuery] string? search, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, [FromQuery] bool manage = false, CancellationToken ct = default)
    {
        await service.DemandDirectory(Actor, ct);
        if (page < 1 || page > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        var q = manage ? AssetOperationsService.Scoped(db.Rooms.AsNoTracking(),await access.GetProfile(Actor,ct),"resources.manage") : db.Rooms.AsNoTracking().Where(x => x.Active);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search) || x.Code.Contains(search));
        var total = await q.CountAsync(ct);
        var rows = await q.OrderBy(x => x.Code).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new { items = rows.Select(RoomService.View), total, page, pageSize });
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(SaveRoom input, CancellationToken ct) => Ok(await service.SaveRoom(Actor, null, input, ct));
    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(string id, SaveRoom input, CancellationToken ct) => Ok(await service.SaveRoom(Actor, id, input, ct));

    [HttpGet("rooms/{id}/availability")]
    public async Task<IActionResult> Availability(string id, [FromQuery] DateOnly date, CancellationToken ct) =>
        Ok(new { roomId = id, date, slots = await service.Availability(Actor, id, date, ct) });

    [HttpGet("room-requests")]
    public async Task<IActionResult> Requests([FromQuery] bool review = false, [FromQuery] string? status = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool logReview = false, CancellationToken ct = default)
    {
        if (page < 1 || page > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        var p = await access.GetProfile(Actor, ct);
        var permission = logReview ? "logs.review" : review ? "requests.review" : "rooms.book";
        if (!AccessRules.HasPermission(p, permission)) throw AppException.Forbidden();
        var q = db.RoomRequests.AsNoTracking();
        if (!review && !logReview) q = q.Where(x => x.OwnerId == Actor);
        if(logReview) q = q.Where(x => x.Status == "approved");
        var deps = p.DepartmentIds.ToArray();
        q = p.Scope switch
        {
            Scopes.All => q,
            Scopes.Unit => q.Where(x => x.DepartmentId == p.DepartmentId),
            Scopes.Units => q.Where(x => deps.Contains(x.DepartmentId)),
            Scopes.Self => q.Where(x => x.OwnerId == Actor),
            _ => q.Where(x => false)
        };
        var grants = p.Grants.Where(x => x.Permissions.Contains(permission)).ToArray();
        var all = grants.Any(x => x.Scope == Scopes.All);
        var unit = grants.Any(x => x.Scope == Scopes.Unit);
        var self = grants.Any(x => x.Scope == Scopes.Self);
        q = q.Where(x => all || unit && x.DepartmentId == p.DepartmentId || self && x.OwnerId == Actor);
        if (review) q = q.Where(x => x.Status != RoomRequestStatuses.Draft);
        if (status != null) q = q.Where(x => x.Status == status);
        var total = await q.CountAsync(ct);
        var rows = await q.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Ok(new { items = rows.Select(RoomService.View), total, page, pageSize });
    }

    [HttpGet("room-requests/{id}")]
    public async Task<IActionResult> RequestDetail(string id, [FromQuery] bool review = false, CancellationToken ct = default) =>
        Ok(RoomService.View(await service.RequestAccess(Actor, id, review, ct)));

    [HttpPost("room-requests")]
    public async Task<IActionResult> Create(SaveRoomRequest input, CancellationToken ct) => Ok(await service.Create(Actor, Key, input, ct));
    [HttpPut("room-requests/{id}")]
    public async Task<IActionResult> Update(string id, SaveRoomRequest input, CancellationToken ct) => Ok(await service.UpdateDraft(Actor, id, input, ct));
    [HttpPost("room-requests/{id}/{operation:regex(^(submit|approve|reject|cancel)$)}")]
    public async Task<IActionResult> Act(string id, string operation, RoomAction input, CancellationToken ct) => Ok(await service.Act(Actor, id, operation, Key, input, ct));

    [HttpGet("rooms/{id}/blocks")]
    public async Task<IActionResult> Blocks(string id, CancellationToken ct)
    {
        await service.DemandManageRoom(Actor, id, ct);
        return Ok(await db.RoomReservations.AsNoTracking().Where(x => x.RoomId == id && x.Kind == "maintenance")
            .OrderByDescending(x => x.Start).Take(100).Select(x => new RoomBlockView(x.Id, x.RoomId, x.Start, x.End, x.Active, Convert.ToBase64String(x.RowVersion))).ToArrayAsync(ct));
    }
    [HttpPost("rooms/{id}/blocks")]
    public async Task<IActionResult> Block(string id, SaveRoomBlock input, CancellationToken ct) => Ok(await service.Block(Actor, id, Key, input, ct));
    [HttpPost("room-blocks/{id}/cancel")]
    public async Task<IActionResult> CancelBlock(string id, RoomAction input, CancellationToken ct) => Ok(await service.CancelBlock(Actor, id, Key, input, ct));
}
