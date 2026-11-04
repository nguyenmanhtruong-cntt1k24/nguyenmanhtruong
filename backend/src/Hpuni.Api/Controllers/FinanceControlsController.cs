using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1/finance"),Authorize]
public class FinanceControlsController(HpuniDbContext db,FinanceControlService service,IAccessService access):ControllerBase
{
    string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    string Key=>Request.Headers["Idempotency-Key"].ToString();
    [HttpGet("period")]
    public async Task<IActionResult> Period([FromQuery]string departmentId,[FromQuery]int year,CancellationToken ct)=>Ok(await service.Period(Actor,departmentId,year,ct));
    [HttpPost("period")]
    public async Task<IActionResult> ChangePeriod(ChangeFiscalPeriod input,CancellationToken ct)=>Ok(await service.ChangePeriod(Actor,Key,input,ct));
    [HttpGet("adjustments")]
    public async Task<IActionResult> Adjustments([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]int? year=null,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,CancellationToken ct=default)
    {
        if(page is <1 or >100000||pageSize is <1 or >100)throw AppException.Invalid("Phân trang không hợp lệ.");
        if(year is <2000 or >2100)throw AppException.Invalid("Năm tài chính phải từ 2000 đến 2100.");
        if(!string.IsNullOrEmpty(category)&&!FinanceService.Categories.Contains(category))throw AppException.Invalid("Khoản mục ngân sách không hợp lệ.");
        var profile=await access.GetProfile(Actor,ct);
        var q=AssetOperationsService.Scoped(db.BudgetAdjustments.AsNoTracking(),profile,"budget.read");
        if(year!=null)q=q.Where(x=>x.Year==year);
        if(!string.IsNullOrEmpty(departmentId)){
            if(!Hpuni.Domain.AccessRules.Allows(profile,"budget.read",departmentId,null))throw AppException.Forbidden();
            q=q.Where(x=>x.DepartmentId==departmentId);
        }
        if(!string.IsNullOrEmpty(category))q=q.Where(x=>x.Category==category);
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=rows.Select(FinanceControlService.View),total,page,pageSize});
    }
    [HttpPost("adjustments")]
    public async Task<IActionResult> Create(CreateBudgetAdjustment input,CancellationToken ct)=>Ok(await service.CreateAdjustment(Actor,Key,input,ct));
    [HttpPost("adjustments/{id}/review")]
    public async Task<IActionResult> Review(string id,ReviewBudgetAdjustment input,CancellationToken ct)=>Ok(await service.ReviewAdjustment(Actor,id,Key,input,ct));
    [HttpPost("entries/{id}/reverse")]
    public async Task<IActionResult> Reverse(string id,ReverseFinanceEntry input,CancellationToken ct)=>Ok(await service.Reverse(Actor,id,Key,input,ct));
    [HttpGet("reversals")]
    public async Task<IActionResult> Reversals([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? entryIds=null,CancellationToken ct=default)
    {
        if(page is <1 or >100000||pageSize is <1 or >100)throw AppException.Invalid("Phân trang không hợp lệ.");
        var entries=AssetOperationsService.Scoped(db.FinanceEntries.AsNoTracking(),await access.GetProfile(Actor,ct),"budget.read");
        var q=from r in db.FinanceReversals.AsNoTracking() join e in entries on r.EntryId equals e.Id select r;
        if(entryIds!=null){var ids=entryIds.Split(',',StringSplitOptions.RemoveEmptyEntries);if(ids.Length>100)throw AppException.Invalid("Tối đa 100 mã chứng từ.");q=q.Where(x=>ids.Contains(x.EntryId));}
        var total=await q.CountAsync(ct);var items=await q.OrderByDescending(x=>x.CreatedAt).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).Select(r=>new FinanceReversalView(r.Id,r.EntryId,r.Reason,r.ActorId,r.CreatedAt)).ToArrayAsync(ct);
        return Ok(new{items,total,page,pageSize});
    }
}
