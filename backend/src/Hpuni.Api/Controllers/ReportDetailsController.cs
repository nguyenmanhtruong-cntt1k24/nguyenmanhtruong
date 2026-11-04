using System.Security.Claims;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1/reports/details"), Authorize]
public class ReportDetailsController(ReportDetailsService service, AdminService admin, HpuniDbContext db) : ControllerBase
{
    string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("{kind:regex(^(finance|assets|loans|rooms)$)}")]
    public async Task<IActionResult> Read(string kind, [FromQuery] int year, [FromQuery] string? departmentId = null,
        [FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await service.Read(Actor, kind, year, departmentId, category, page, pageSize, false, ct));
    [HttpGet("{kind:regex(^(finance|assets|loans|rooms)$)}/export/{format:regex(^(csv|xlsx|print)$)}")]
    public async Task<IActionResult> Export(string kind, string format, [FromQuery] int year, [FromQuery] string? departmentId = null,
        [FromQuery] string? category = null, CancellationToken ct = default)
    {
        var report = await service.Read(Actor, kind, year, departmentId, category, 1, 20, true, ct);
        var content = format switch { "csv" => ReportDetailsService.Csv(report), "xlsx" => ReportDetailsService.Excel(report), _ => null };
        admin.Audit(Actor, "reports.export.details", kind, new { year, departmentId, category, format, report.Total, report.GeneratedAt });
        await db.SaveChangesAsync(ct);
        if (format == "print")
        {
            Response.Headers.ContentSecurityPolicy = "default-src 'none'; style-src 'unsafe-inline'; base-uri 'none'; frame-ancestors 'none'";
            return Content(ReportDetailsService.PrintHtml(report), "text/html; charset=utf-8");
        }
        return File(content!, format == "xlsx" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv; charset=utf-8", $"chi-tiet-{kind}-{year}.{format}");
    }
}
