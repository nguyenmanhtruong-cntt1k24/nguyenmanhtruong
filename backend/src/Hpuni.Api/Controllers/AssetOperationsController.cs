using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1"),Authorize]
public class AssetOperationsController(HpuniDbContext db,AssetOperationsService service,IAccessService access):ControllerBase
{
    private string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string Key=>Request.Headers["Idempotency-Key"].ToString();
    private static void Page(int page,int pageSize){if(page is <1 or >100000||pageSize is <1 or >100)throw AppException.Invalid("Phân trang không hợp lệ.");}
    [HttpGet("maintenance")]
    public async Task<IActionResult> Maintenance([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        Page(page,pageSize);var q=AssetOperationsService.Scoped(db.MaintenanceOrders.AsNoTracking(),await access.GetProfile(Actor,ct),"resources.manage");
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.Start).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        var items=await service.FinancialViews(rows,ct);
        return Ok(new{items,total,page,pageSize});
    }
    [HttpGet("maintenance/{id}")]
    public async Task<IActionResult> MaintenanceDetail(string id,CancellationToken ct)=>Ok(await service.FinancialView(await service.GetMaintenance(Actor,id,ct),ct));
    [HttpPost("maintenance")]
    public async Task<IActionResult> Plan(PlanMaintenance input,CancellationToken ct)=>Ok(await service.Plan(Actor,Key,input,ct));
    [HttpPost("maintenance/{id}/{operation:regex(^(start|cancel|finish)$)}")]
    public async Task<IActionResult> Act(string id,string operation,FinishMaintenance input,CancellationToken ct)=>Ok(await service.MaintenanceAction(Actor,id,operation,Key,input,ct));
    [HttpGet("inventories")]
    public async Task<IActionResult> Inventories([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)
    {
        Page(page,pageSize);var q=AssetOperationsService.Scoped(db.InventorySessions.AsNoTracking(),await access.GetProfile(Actor,ct),"inventory.manage");
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(x=>x.At).ThenBy(x=>x.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=rows.Select(s=>new{s.Id,s.Name,s.DepartmentId,s.Status,s.At,version=Convert.ToBase64String(s.RowVersion)}),total,page,pageSize});
    }
    [HttpPost("inventories")]
    public async Task<IActionResult> Create(CreateInventory input,CancellationToken ct)=>Ok(await service.CreateInventory(Actor,Key,input,ct));
    [HttpGet("inventories/{id}")]
    public async Task<IActionResult> Inventory(string id,CancellationToken ct)=>Ok(await service.InventoryView(await service.GetInventory(Actor,id,ct),ct));
    [HttpPost("inventories/{id}/count")]
    public async Task<IActionResult> Count(string id,SaveInventory input,CancellationToken ct)=>Ok(await service.SaveInventory(Actor,id,Key,input,ct));
    [HttpGet("equipment-loans/{id}/log")]
    public async Task<IActionResult> Log(string id,[FromQuery]bool review=false,CancellationToken ct=default)=>Ok(await service.GetLog(Actor,id,review,ct));
    [HttpPost("equipment-loans/{id}/log")]
    public async Task<IActionResult> SaveLog(string id,SaveEquipmentLog input,CancellationToken ct)=>Ok(await service.SaveLog(Actor,id,Key,input,ct));
    [HttpPost("equipment-loans/{id}/log/review")]
    public async Task<IActionResult> Review(string id,ReviewRoomLog input,CancellationToken ct)=>Ok(await service.ReviewLog(Actor,id,Key,input,ct));
}
