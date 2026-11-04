using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

public record ResetPasswordInput([Required, MaxLength(128)] string Password, [Required] string Version);

[ApiController, Route("api/v1"), Authorize]
public class AdminController(HpuniDbContext db, AdminService admin, IAccessService access) : ControllerBase
{
    private string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("users")]
    public async Task<IActionResult> Users([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? departmentId = null,
        [FromQuery] string? status = null, [FromQuery] string? roleId = null,
        [FromQuery] bool includeSummary = false, CancellationToken ct = default)
    {
        if (page < 1 || page > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        var p = await access.GetProfile(Actor, ct);
        if (!AccessRules.HasPermission(p, "users.manage")) throw AppException.Forbidden();
        var query = AccessQueries.Users(db.Users.AsNoTracking(), p, "users.manage");
        object? summary = null;
        if (includeSummary)
        {
            var scopedIds = query.Select(x => x.Id);
            summary = new
            {
                total = await query.CountAsync(ct), active = await query.CountAsync(x => x.Active, ct),
                roles = await db.UserRoles.AsNoTracking().Where(x => scopedIds.Contains(x.UserId))
                    .GroupBy(x => x.RoleId).Select(g => new { roleId = g.Key, count = g.Count() }).ToArrayAsync(ct),
                departments = await query.GroupBy(x => x.DepartmentId)
                    .Select(g => new { departmentId = g.Key, count = g.Count() }).ToArrayAsync(ct)
            };
        }
        search = search?.Trim();
        if (search?.Length > 150) throw AppException.Invalid("Từ khóa tối đa 150 ký tự.");
        if (!string.IsNullOrEmpty(search)) query = query.Where(x => x.UserName!.Contains(search) || x.Email!.Contains(search) || x.DisplayName.Contains(search));
        if (!string.IsNullOrEmpty(departmentId)) query = query.Where(x => x.DepartmentId == departmentId);
        if (!string.IsNullOrEmpty(status))
        {
            if (status is not ("active" or "locked")) throw AppException.Invalid("Trạng thái tài khoản không hợp lệ.");
            query = query.Where(x => x.Active == (status == "active"));
        }
        if (!string.IsNullOrEmpty(roleId)) query = query.Where(x => db.UserRoles.Any(r => r.UserId == x.Id && r.RoleId == roleId));
        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(x => x.UserName)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var result = new List<UserView>();
        foreach (var u in rows) result.Add(await admin.View(u, ct));
        return Ok(new { items = result, total, page, pageSize, summary });
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateUser input, CancellationToken ct) => Ok(await admin.CreateUser(Actor, input, ct));

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUser input, CancellationToken ct) => Ok(await admin.UpdateUser(Actor, id, input, ct));

    [HttpPost("users/{id}/status")]
    public async Task<IActionResult> Status(string id, ChangeStatus input, CancellationToken ct) => Ok(await admin.Status(Actor, id, input, ct));

    [HttpPut("users/{id}/scope")]
    public async Task<IActionResult> Scope(string id, ConfigureScope input, CancellationToken ct) => Ok(await admin.Scope(Actor, id, input, ct));

    [HttpPut("users/{id}/role-overrides")]
    public async Task<IActionResult> Override(string id, ConfigureOverride input, CancellationToken ct) => Ok(await admin.Override(Actor, id, input, ct));

    [HttpPost("users/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(string id, ResetPasswordInput input, CancellationToken ct)
    {
        await admin.ResetPassword(Actor, id, input.Password, input.Version, ct);
        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles(CancellationToken ct)
    {
        await admin.DemandGlobal(Actor, "roles.manage", ct);
        var rows = await db.Roles.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
        var result = new List<RoleView>();
        foreach (var r in rows) result.Add(await admin.View(r, ct));
        return Ok(result);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole(SaveRole input, CancellationToken ct) => Ok(await admin.SaveRole(Actor, null, input, ct));
    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(string id, SaveRole input, CancellationToken ct) => Ok(await admin.SaveRole(Actor, id, input, ct));

    [HttpGet("permissions")]
    public async Task<IActionResult> PermissionsList(CancellationToken ct)
    {
        await admin.DemandGlobal(Actor, "permissions.manage", ct);
        return Ok(Permissions.All);
    }

    [HttpGet("departments")]
    public async Task<IActionResult> Departments(CancellationToken ct)
    {
        // The active unit directory is shared by authenticated users; inactive entries require management rights.
        var p = await access.GetProfile(Actor, ct);
        var manage = AccessRules.IsGlobal(p, "departments.manage");
        var rows = await db.Departments.AsNoTracking().Where(x => manage || x.Active).OrderBy(x => x.Name).ToListAsync(ct);
        return Ok(rows.Select(AdminService.View));
    }

    [HttpPost("departments")]
    public async Task<IActionResult> CreateDepartment(SaveDepartment input, CancellationToken ct) => Ok(await admin.SaveDepartment(Actor, null, input, ct));
    [HttpPut("departments/{id}")]
    public async Task<IActionResult> UpdateDepartment(string id, SaveDepartment input, CancellationToken ct) => Ok(await admin.SaveDepartment(Actor, id, input, ct));

    [HttpGet("audit-events")]
    public async Task<IActionResult> Audit([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        await admin.DemandGlobal(Actor, "audit.read", ct);
        if (page < 1 || page > 100000 || pageSize is < 1 or > 100) throw AppException.Invalid("Phân trang không hợp lệ.");
        return Ok(new { items = await db.AuditEvents.AsNoTracking().OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct), total = await db.AuditEvents.CountAsync(ct), page, pageSize });
    }
}
