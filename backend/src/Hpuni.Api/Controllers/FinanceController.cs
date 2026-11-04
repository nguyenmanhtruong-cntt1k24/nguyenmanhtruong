using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1/finance"),Authorize]
public class FinanceController(HpuniDbContext db,FinanceService service,IAccessService access):ControllerBase
{
    string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    string Key=>Request.Headers["Idempotency-Key"].ToString();
    static void Page(int page,int size){if(page is <1 or >100000||size is <1 or >100)throw AppException.Invalid("Phân trang không hợp lệ.");}
    [HttpGet("proposals")]
    public async Task<IActionResult> Proposals([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]int? year=null,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,CancellationToken ct=default)
    {
        Page(page,pageSize);var q=AssetOperationsService.Scoped(db.BudgetProposals.AsNoTracking(),await access.GetProfile(Actor,ct),"budget.read");
        if(year!=null)q=q.Where(x=>x.Year==year);if(!string.IsNullOrEmpty(departmentId)){await service.Demand(Actor,"budget.read",departmentId,ct);q=q.Where(x=>x.DepartmentId==departmentId);}if(!string.IsNullOrEmpty(category))q=q.Where(x=>x.Category==category);
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=rows.Select(FinanceService.View),total,page,pageSize});
    }
    [HttpGet("proposals/{id}")]
    public async Task<IActionResult> Proposal(string id,CancellationToken ct)=>Ok(FinanceService.View(await service.GetProposal(Actor,id,"budget.read",ct)));
    [HttpPost("proposals")]
    public async Task<IActionResult> Create(SaveProposal input,CancellationToken ct)=>Ok(await service.Save(Actor,null,Key,input,ct));
    [HttpPut("proposals/{id}")]
    public async Task<IActionResult> Save(string id,SaveProposal input,CancellationToken ct)=>Ok(await service.Save(Actor,id,Key,input,ct));
    [HttpPost("proposals/{id}/{operation:regex(^(submit|approve|reject)$)}")]
    public async Task<IActionResult> Act(string id,string operation,ProposalCommand input,CancellationToken ct)=>Ok(await service.Action(Actor,id,operation,Key,input,ct));
    [HttpGet("balance")]
    public async Task<IActionResult> Balance([FromQuery]string departmentId,[FromQuery]int year,[FromQuery]string category="general",CancellationToken ct=default)=>Ok(await service.Balance(Actor,departmentId,year,category,ct));
    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery]int year,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,CancellationToken ct=default)=>Ok(await service.Summary(Actor,year,departmentId,category,ct));
    [HttpGet("categories")]
    public async Task<IActionResult> Categories(CancellationToken ct)
    {
        var p=await access.GetProfile(Actor,ct);if(!Hpuni.Domain.AccessRules.HasPermission(p,"budget.read"))throw AppException.Forbidden();
        return Ok(FinanceService.Categories);
    }
    [HttpGet("payments")]
    public async Task<IActionResult> Payments([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]int? year=null,[FromQuery]string? departmentId=null,CancellationToken ct=default)
    {
        Page(page,pageSize);var q=AssetOperationsService.Scoped(db.PaymentRequests.AsNoTracking(),await access.GetProfile(Actor,ct),"budget.read");
        if(year!=null)q=q.Where(x=>x.Year==year);if(!string.IsNullOrEmpty(departmentId)){await service.Demand(Actor,"budget.read",departmentId,ct);q=q.Where(x=>x.DepartmentId==departmentId);}
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=await service.PaymentViews(rows,ct),total,page,pageSize});
    }
    [HttpPost("payments")]
    public async Task<IActionResult> Payment(CreatePaymentRequest input,CancellationToken ct)=>Ok(await service.CreatePayment(Actor,Key,input,ct));
    [HttpGet("payments/{id}")]
    public async Task<IActionResult> PaymentById(string id,CancellationToken ct)
    {
        var payment=await db.PaymentRequests.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await service.Demand(Actor,"budget.read",payment.DepartmentId,ct);
        return Ok((await service.PaymentViews([payment],ct)).Single());
    }
    [HttpPost("payments/from-maintenance/{id}")]
    public async Task<IActionResult> Import(string id,ImportMaintenanceCost input,CancellationToken ct)=>Ok(await service.ImportMaintenance(Actor,id,Key,input,ct));
    [HttpGet("entries/{id}")]
    public async Task<IActionResult> Entry(string id,CancellationToken ct)
    {
        var e=await db.FinanceEntries.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await service.Demand(Actor,"budget.read",e.DepartmentId,ct);return Ok(FinanceService.View(e));
    }
    [HttpGet("entries")]
    public async Task<IActionResult> Entries([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]int? year=null,[FromQuery]string? departmentId=null,[FromQuery]string? category=null,[FromQuery]string? paymentRequestId=null,CancellationToken ct=default)
    {
        Page(page,pageSize);var q=AssetOperationsService.Scoped(db.FinanceEntries.AsNoTracking(),await access.GetProfile(Actor,ct),"budget.read");
        if(year!=null)q=q.Where(x=>x.Year==year);if(!string.IsNullOrEmpty(departmentId)){await service.Demand(Actor,"budget.read",departmentId,ct);q=q.Where(x=>x.DepartmentId==departmentId);}if(!string.IsNullOrEmpty(category))q=q.Where(x=>x.Category==category);if(!string.IsNullOrEmpty(paymentRequestId))q=q.Where(x=>x.PaymentRequestId==paymentRequestId);
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=rows.Select(FinanceService.View),total,page,pageSize});
    }
    [HttpPost("entries/{kind:regex(^(receipt|expense)$)}")]
    public async Task<IActionResult> Record(string kind,RecordFinanceEntry input,CancellationToken ct)=>Ok(await service.Record(Actor,kind,Key,input,ct));
}
