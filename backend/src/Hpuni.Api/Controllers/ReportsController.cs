using System.Security.Claims;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1/reports"),Authorize]
public class ReportsController(ReportService service,AdminService admin,HpuniDbContext db):ControllerBase
{
    string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("finance")]
    public async Task<IActionResult> Finance([FromQuery]int year,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,CancellationToken ct=default)=>Ok(await service.Finance(Actor,year,departmentId,category,false,ct));
    [HttpGet("resources")]
    public async Task<IActionResult> Resources([FromQuery]int year,[FromQuery]string? departmentId=null,CancellationToken ct=default)=>Ok(await service.Resources(Actor,year,departmentId,false,ct));
    [HttpGet("finance.csv")]
    public async Task<IActionResult> FinanceCsv([FromQuery]int year,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,CancellationToken ct=default)
    {
        var report=await service.Finance(Actor,year,departmentId,category,true,ct);
        admin.Audit(Actor,"reports.export.finance",year.ToString(),new{departmentId,category,rows=report.Rows.Length,report.GeneratedAt});await db.SaveChangesAsync(ct);
        return File(ReportService.FinanceCsv(report),"text/csv; charset=utf-8",$"tai-chinh-{year}.csv");
    }
    [HttpGet("resources.csv")]
    public async Task<IActionResult> ResourceCsv([FromQuery]int year,[FromQuery]string? departmentId=null,CancellationToken ct=default)
    {
        var report=await service.Resources(Actor,year,departmentId,true,ct);
        admin.Audit(Actor,"reports.export.resources",year.ToString(),new{departmentId,rows=report.Rows.Length,report.GeneratedAt});await db.SaveChangesAsync(ct);
        return File(ReportService.ResourceCsv(report),"text/csv; charset=utf-8",$"tai-san-phong-{year}.csv");
    }
}
