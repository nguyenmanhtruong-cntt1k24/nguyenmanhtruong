using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1"), Authorize]
public class EquipmentController(HpuniDbContext db, EquipmentService service, IAccessService access) : ControllerBase
{
    private string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string Key => Request.Headers["Idempotency-Key"].ToString();
    private static void Page(int page, int pageSize)
    {
        if (page is < 1 or > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
    }
    [HttpGet("equipment")]
    public async Task<IActionResult> Assets([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] bool manage = false, CancellationToken ct = default)
    {
        await service.Directory(Actor, ct); Page(page, pageSize);
        var q = manage ? AssetOperationsService.Scoped(db.AssetGroups.AsNoTracking(),await access.GetProfile(Actor,ct),"resources.manage") : db.AssetGroups.AsNoTracking().Where(x => x.Active);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Name.Contains(search) || x.Code.Contains(search));
        var total = await q.CountAsync(ct);
        var rows = await q.OrderBy(x => x.Code).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new { items = await service.AssetViews(rows, ct), total, page, pageSize });
    }
    [HttpPost("equipment")]
    public async Task<IActionResult> CreateAsset(SaveAssetGroup input, CancellationToken ct) => Ok(await service.CreateAsset(Actor, Key, input, ct));
    [HttpGet("equipment/{id}")]
    public async Task<IActionResult> Asset(string id, CancellationToken ct)
    {
        await service.Directory(Actor, ct);
        var a = await db.AssetGroups.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.Active, ct) ?? throw AppException.Missing();
        return Ok((await service.AssetViews([a], ct))[0]);
    }
    [HttpPut("equipment/{id}")]
    public async Task<IActionResult> UpdateAsset(string id, SaveAssetGroup input, CancellationToken ct) => Ok(await service.UpdateAsset(Actor, id, input, ct));
    [HttpPost("equipment/{id}/stock")]
    public async Task<IActionResult> Stock(string id, ReceiveStock input, CancellationToken ct) => Ok(await service.Receive(Actor, id, Key, input, ct));
    [HttpGet("equipment/{id}/availability")]
    public async Task<IActionResult> Availability(string id, [FromQuery] DateTimeOffset start, [FromQuery] DateTimeOffset end, CancellationToken ct) =>
        Ok(await service.Availability(Actor, id, start, end, ct));
    [HttpGet("equipment/{id}/movements")]
    public async Task<IActionResult> Movements(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        Page(page, pageSize);
        var a = await db.AssetGroups.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        if (!AccessRules.Allows(await access.GetProfile(Actor, ct), "resources.manage", a.DepartmentId, null)) throw AppException.Forbidden();
        var q = db.EquipmentMovements.AsNoTracking().Where(x => x.AssetGroupId == id);
        return Ok(new { total = await q.CountAsync(ct), page, pageSize,
            items = await q.OrderByDescending(x => x.At).ThenBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct) });
    }
    [HttpGet("equipment-loans")]
    public async Task<IActionResult> Loans([FromQuery] bool review = false, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, [FromQuery] string? status = null, [FromQuery] bool logReview = false, CancellationToken ct = default)
    {
        Page(page, pageSize); var p = await access.GetProfile(Actor, ct);
        if (logReview ? !AccessRules.HasPermission(p,"logs.review") : review ? !AccessRules.HasPermission(p, "requests.review") && !AccessRules.HasPermission(p, "resources.manage") : !AccessRules.HasPermission(p, "devices.borrow"))
            throw AppException.Forbidden();
        var q = logReview ? service.VisibleLogs(p) : service.Visible(p, review);
        if (status != null) q = q.Where(x => x.Status == status);
        var total = await q.CountAsync(ct);
        var rows = await q.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        var items = await service.Views(rows, ct);
        return Ok(new { items, total, page, pageSize });
    }
    [HttpGet("equipment-loans/{id}")]
    public async Task<IActionResult> Loan(string id, [FromQuery] bool review = false, CancellationToken ct = default)
    {
        var r = await service.Get(Actor, id, review, ct); return Ok(await service.View(r, r.Asset, ct));
    }
    [HttpPost("equipment-loans")]
    public async Task<IActionResult> Create(SaveEquipmentLoan input, CancellationToken ct) => Ok(await service.Create(Actor, Key, input, ct));
    [HttpPut("equipment-loans/{id}")]
    public async Task<IActionResult> UpdateDraft(string id, SaveEquipmentLoan input, CancellationToken ct) =>
        Ok(await service.UpdateDraft(Actor, id, input, ct));
    [HttpPost("equipment-loans/{id}/{operation:regex(^(submit|approve|reject|cancel|handover|receipt|resolve-receipt)$)}")]
    public async Task<IActionResult> Act(string id, string operation, EquipmentCommand input, CancellationToken ct) => Ok(await service.Act(Actor, id, operation, Key, input, ct));
    [HttpPost("equipment-loans/{id}/returns")]
    public async Task<IActionResult> Return(string id, ReturnEquipment input, CancellationToken ct) => Ok(await service.RequestReturn(Actor, id, Key, input, ct));
    [HttpPost("equipment-loans/{id}/accept-return")]
    public async Task<IActionResult> AcceptReturn(string id, AcceptEquipmentReturn input, CancellationToken ct) => Ok(await service.AcceptReturn(Actor, id, Key, input, ct));
}
