using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1/asset-lifecycle"), Authorize]
public class AssetLifecycleController(HpuniDbContext db, IAccessService access, AssetLifecycleService service) : ControllerBase
{
    string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    string Key => Request.Headers["Idempotency-Key"].ToString();
    [HttpGet("assets")]
    public async Task<IActionResult> Assets([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        if (page is < 1 or > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        var q = AssetLifecycleService.VisibleAssets(db.AssetGroups.AsNoTracking(), await access.GetProfile(Actor, ct));
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
        var total = await q.CountAsync(ct); var items = await q.OrderBy(x => x.Code).ThenBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.Code, x.Name, x.DepartmentId, x.Total, x.Damaged, x.Lost, x.Active }).ToArrayAsync(ct);
        return Ok(new { items, total, page, pageSize });
    }
    [HttpGet("profiles/{assetId}")]
    public async Task<IActionResult> Profile(string assetId, CancellationToken ct) => Ok(await service.Profile(Actor, assetId, ct));
    [HttpGet("assets/{assetId}")]
    public async Task<IActionResult> Asset(string assetId, CancellationToken ct)
    {
        var q = AssetLifecycleService.VisibleAssets(db.AssetGroups.AsNoTracking(), await access.GetProfile(Actor, ct));
        var item = await q.Where(x => x.Id == assetId).Select(x => new { x.Id, x.Code, x.Name, x.DepartmentId, x.Total, x.Damaged, x.Lost, x.Active }).SingleOrDefaultAsync(ct) ?? throw AppException.Missing();
        return Ok(item);
    }
    [HttpPost("profiles/{assetId}")]
    public async Task<IActionResult> SaveProfile(string assetId, SaveFixedAssetProfile input, CancellationToken ct) => Ok(await service.SaveProfile(Actor, assetId, Key, input, ct));
    [HttpGet("disposals")]
    public async Task<IActionResult> List([FromQuery] string? assetGroupId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        if (page is < 1 or > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        if (status != null && status is not ("pending" or "approved" or "rejected")) throw AppException.Invalid("Trạng thái thanh lý không hợp lệ.");
        if (search?.Length > 150) throw AppException.Invalid("Nội dung tìm kiếm tối đa 150 ký tự.");
        if (assetGroupId != null) await service.Profile(Actor, assetGroupId, ct);
        var q = AssetLifecycleService.Visible(db.Set<AssetDisposal>().AsNoTracking(), await access.GetProfile(Actor, ct));
        if (assetGroupId != null) q = q.Where(x => x.AssetGroupId == assetGroupId);
        if (status != null) q = q.Where(x => x.Status == status);
        var joined = from d in q join a in db.AssetGroups on d.AssetGroupId equals a.Id select new { Disposal = d, a.Code, a.Name };
        if (!string.IsNullOrWhiteSpace(search)) joined = joined.Where(x => x.Code.Contains(search) || x.Name.Contains(search) || x.Disposal.Reference.Contains(search));
        var total = await joined.CountAsync(ct); var rows = await joined.OrderByDescending(x => x.Disposal.CreatedAt).ThenBy(x => x.Disposal.Id).Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new { items = rows.Select(x => AssetLifecycleService.View(x.Disposal) with { AssetCode = x.Code, AssetName = x.Name }), total, page, pageSize });
    }
    [HttpGet("disposals/{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct) => Ok(await service.Get(Actor, id, ct));
    [HttpPost("disposals")]
    public async Task<IActionResult> Create(CreateAssetDisposal input, CancellationToken ct) => Ok(await service.Create(Actor, Key, input, ct));
    [HttpPost("disposals/{id}/{operation:regex(^(approve|reject)$)}")]
    public async Task<IActionResult> Review(string id, string operation, ReviewAssetDisposal input, CancellationToken ct) => Ok(await service.Review(Actor, id, operation, Key, input, ct));
}
