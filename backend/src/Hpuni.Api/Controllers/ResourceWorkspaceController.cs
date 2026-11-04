using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1/resource-workspace"), Authorize]
public class ResourceWorkspaceController(HpuniDbContext db, IAccessService access, EquipmentService equipment,
    AssetOperationsService operations) : ControllerBase
{
    string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    static void Page(int page,int pageSize,string? search) { if(page is <1 or >100000 || pageSize is <1 or >100 || search?.Length>150) throw AppException.Invalid("Bộ lọc hoặc phân trang không hợp lệ."); }
    static bool Has(AccessProfile p,string permission)=>AccessRules.HasPermission(p,permission);
    IQueryable<RoomRequest> RoomsFor(AccessProfile p,string permission)
    {
        if(!Has(p,permission))return db.RoomRequests.AsNoTracking().Where(r=>false);
        var deps=p.DepartmentIds.ToArray();var grants=p.Grants.Where(g=>g.Permissions.Contains(permission)).ToArray();
        var all=grants.Any(g=>g.Scope=="all");var unit=grants.Any(g=>g.Scope=="unit");var self=grants.Any(g=>g.Scope=="self");
        return db.RoomRequests.AsNoTracking().Where(r=>(p.Scope=="all"||p.Scope=="unit"&&r.DepartmentId==p.DepartmentId||p.Scope=="units"&&deps.Contains(r.DepartmentId)||p.Scope=="self"&&r.OwnerId==p.UserId)
            &&(all||unit&&r.DepartmentId==p.DepartmentId||self&&r.OwnerId==p.UserId));
    }
    IQueryable<EquipmentLoan> LoansFor(AccessProfile p)=>Has(p,"requests.review")||Has(p,"resources.manage")?equipment.Visible(p,true):db.EquipmentLoans.AsNoTracking().Where(r=>false);
    IQueryable<EquipmentLoan> LogsLoans(AccessProfile p)=>Has(p,"logs.review")?equipment.VisibleLogs(p):db.EquipmentLoans.AsNoTracking().Where(r=>false);

    [HttpGet("rooms")]
    public async Task<IActionResult> Rooms([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,
        [FromQuery]string? departmentId=null,[FromQuery]string? status=null,[FromQuery]string? roomId=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);
        if(!Has(p,"resources.manage")&&!Has(p,"requests.review"))throw AppException.Forbidden();
        var q=Has(p,"resources.manage")?AssetOperationsService.Scoped(db.Rooms.AsNoTracking(),p,"resources.manage"):db.Rooms.AsNoTracking().Where(r=>r.Active);
        if(!string.IsNullOrEmpty(search))q=q.Where(r=>r.Code.Contains(search)||r.Name.Contains(search));
        if(!string.IsNullOrEmpty(departmentId))q=q.Where(r=>r.DepartmentId==departmentId);
        if(!string.IsNullOrEmpty(roomId))q=q.Where(r=>r.Id==roomId);
        if(status is "true" or "false")q=q.Where(r=>r.Active==(status=="true"));
        var total=await q.CountAsync(ct);var rows=await q.OrderBy(r=>r.Code).ThenBy(r=>r.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=rows.Select(RoomService.View),total,page,pageSize});
    }
    [HttpGet("assets")]
    public async Task<IActionResult> Assets([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,
        [FromQuery]string? departmentId=null,[FromQuery]string? status=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);
        var q=AssetOperationsService.Scoped(db.AssetGroups.AsNoTracking(),p,"resources.manage");
        if(!string.IsNullOrEmpty(search))q=q.Where(a=>a.Code.Contains(search)||a.Name.Contains(search));
        if(!string.IsNullOrEmpty(departmentId))q=q.Where(a=>a.DepartmentId==departmentId);
        if(status=="active")q=q.Where(a=>a.Active);else if(status=="inactive")q=q.Where(a=>!a.Active);
        else if(status=="issue")q=q.Where(a=>a.Damaged+a.Lost>0);else if(status=="good")q=q.Where(a=>a.Damaged+a.Lost==0);
        var total=await q.CountAsync(ct);var rows=await q.OrderBy(a=>a.Code).ThenBy(a=>a.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        return Ok(new{items=await equipment.AssetViews(rows,ct),total,page,pageSize});
    }
    [HttpGet("requests")]
    public async Task<IActionResult> Requests([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,
        [FromQuery]string? status=null,[FromQuery]string? kind=null,[FromQuery]DateOnly? from=null,[FromQuery]DateOnly? to=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);
        if(!Has(p,"requests.review")&&!Has(p,"resources.manage"))throw AppException.Forbidden();
        var rooms=RoomsFor(p,"requests.review").Where(r=>r.Status!="draft");var loans=LoansFor(p);
        if(kind=="room")loans=loans.Where(r=>false);else if(kind=="device")rooms=rooms.Where(r=>false);
        if(!string.IsNullOrEmpty(status)) { if(status=="overdue") { rooms=rooms.Where(r=>false);var now=DateTimeOffset.UtcNow;loans=loans.Where(r=>r.End<now&&r.Returned<r.Quantity&&(r.Status=="borrowed"||r.Status=="return_pending")); }
            else { rooms=rooms.Where(r=>r.Status==status);loans=loans.Where(r=>r.Status==status); } }
        if(!string.IsNullOrEmpty(search)){rooms=rooms.Where(r=>r.Id.Contains(search)||r.OwnerId.Contains(search)||r.Purpose.Contains(search)||db.Rooms.Any(a=>a.Id==r.RoomId&&(a.Name.Contains(search)||a.Code.Contains(search))));loans=loans.Where(r=>r.Id.Contains(search)||r.OwnerId.Contains(search)||r.Purpose.Contains(search)||r.Asset.Name.Contains(search)||r.Asset.Code.Contains(search));}
        if(from!=null){var start=new DateTimeOffset(from.Value.ToDateTime(TimeOnly.MinValue),TimeSpan.FromHours(7));rooms=rooms.Where(r=>r.Start>=start);loans=loans.Where(r=>r.Start>=start);}
        if(to!=null){var end=new DateTimeOffset(to.Value.AddDays(1).ToDateTime(TimeOnly.MinValue),TimeSpan.FromHours(7));rooms=rooms.Where(r=>r.Start<end);loans=loans.Where(r=>r.Start<end);}
        var refs=rooms.Select(r=>new{kind="room",r.Id,at=r.CreatedAt}).Concat(loans.Select(r=>new{kind="device",r.Id,at=r.CreatedAt}));
        var total=await refs.CountAsync(ct);var selected=await refs.OrderByDescending(r=>r.at).ThenBy(r=>r.Id).ThenBy(r=>r.kind).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        var roomIds=selected.Where(r=>r.kind=="room").Select(r=>r.Id).ToArray();var loanIds=selected.Where(r=>r.kind=="device").Select(r=>r.Id).ToArray();
        var roomRows=await rooms.Where(r=>roomIds.Contains(r.Id)).ToArrayAsync(ct);var loanRows=await loans.Where(r=>loanIds.Contains(r.Id)).Include(r=>r.Asset).ToArrayAsync(ct);
        var resourceIds=roomRows.Select(r=>r.RoomId).ToArray();var names=await db.Rooms.Where(r=>resourceIds.Contains(r.Id)).ToDictionaryAsync(r=>r.Id,r=>r.Name,ct);
        var items=new Dictionary<string,object>();foreach(var r in roomRows)items["room:"+r.Id]=new{kind="room",request=(object)RoomService.View(r),name=names.GetValueOrDefault(r.RoomId,r.RoomId)};
        var loanViews=(await equipment.Views(loanRows,ct)).ToDictionary(v=>v.Id);
        foreach(var r in loanRows)items["device:"+r.Id]=new{kind="device",request=(object)loanViews[r.Id],name=r.Asset.Name,manageDepartmentId=r.Asset.DepartmentId};
        return Ok(new{items=selected.Select(r=>items[r.kind+":"+r.Id]),total,page,pageSize});
    }
    [HttpGet("logs")]
    public async Task<IActionResult> Logs([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,[FromQuery]string? status=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);if(!Has(p,"logs.review"))throw AppException.Forbidden();
        var rooms=RoomsFor(p,"logs.review").Where(r=>r.Status=="approved");var loans=LogsLoans(p);
        var rq=from l in db.RoomUseLogs.AsNoTracking() join r in rooms on l.RequestId equals r.Id join a in db.Rooms on r.RoomId equals a.Id select new{kind="room",l.Id,requestId=r.Id,at=r.Start,l.Status,name=a.Name};
        var eq=from l in db.EquipmentUseLogs.AsNoTracking() join r in loans on l.LoanId equals r.Id select new{kind="device",l.Id,requestId=r.Id,at=r.Start,l.Status,name=r.Asset.Name};
        var q=rq.Concat(eq);if(!string.IsNullOrEmpty(status)){var apiStatus=status=="pending"?"submitted":status=="needs_update"?"needs_changes":status;q=q.Where(r=>r.Status==apiStatus);}
        if(!string.IsNullOrEmpty(search))q=q.Where(r=>r.name.Contains(search)||r.Id.Contains(search)||r.requestId.Contains(search));
        var total=await q.CountAsync(ct);var selected=await q.OrderByDescending(r=>r.at).ThenBy(r=>r.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        var roomIds=selected.Where(r=>r.kind=="room").Select(r=>r.requestId).ToArray();var loanIds=selected.Where(r=>r.kind=="device").Select(r=>r.requestId).ToArray();
        var roomRows=await rooms.Where(r=>roomIds.Contains(r.Id)).ToDictionaryAsync(r=>r.Id,ct);var loanRows=await loans.Where(r=>loanIds.Contains(r.Id)).Include(r=>r.Asset).ToDictionaryAsync(r=>r.Id,ct);
        var roomLogs=await db.RoomUseLogs.AsNoTracking().Where(l=>roomIds.Contains(l.RequestId)).ToDictionaryAsync(l=>l.RequestId,ct);var loanLogs=await db.EquipmentUseLogs.AsNoTracking().Where(l=>loanIds.Contains(l.LoanId)).ToDictionaryAsync(l=>l.LoanId,ct);
        var loanViews=(await equipment.Views(loanRows.Values.ToArray(),ct)).ToDictionary(v=>v.Id);
        var items=new List<object>();foreach(var r in selected){if(r.kind=="room")items.Add(new{r.kind,r.name,request=(object)RoomService.View(roomRows[r.requestId]),log=(object)RoomLogService.View(roomLogs[r.requestId])});
            else {var loan=loanRows[r.requestId];var l=loanLogs[r.requestId];items.Add(new{r.kind,r.name,manageDepartmentId=loan.Asset.DepartmentId,request=(object)loanViews[loan.Id],log=(object)new EquipmentLogView(l.Id,l.LoanId,l.Used,l.ActualStart,l.ActualEnd,l.Quantity,l.Condition,l.Issue,l.Status,l.ReviewNote,Convert.ToBase64String(l.RowVersion))});}}
        return Ok(new{items,total,page,pageSize});
    }
    [HttpGet("inventories")]
    public async Task<IActionResult> Inventories([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,[FromQuery]string? status=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);var q=AssetOperationsService.Scoped(db.InventorySessions.AsNoTracking(),p,"inventory.manage");
        if(!string.IsNullOrEmpty(search))q=q.Where(i=>i.Name.Contains(search)||i.Id.Contains(search));if(!string.IsNullOrEmpty(status))q=q.Where(i=>i.Status==status);
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(i=>i.At).ThenBy(i=>i.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);var ids=rows.Select(i=>i.Id).ToArray();
        var counts=await db.InventoryItems.Where(i=>ids.Contains(i.SessionId)).GroupBy(i=>i.SessionId).Select(g=>new{id=g.Key,itemCount=g.Count(),counted=g.Count(i=>i.Actual!=null&&i.Condition!="")}).ToDictionaryAsync(g=>g.id,ct);
        return Ok(new{items=rows.Select(i=>new{i.Id,i.Name,i.DepartmentId,i.Status,i.At,version=Convert.ToBase64String(i.RowVersion),itemCount=counts.GetValueOrDefault(i.Id)?.itemCount??0,counted=counts.GetValueOrDefault(i.Id)?.counted??0}),total,page,pageSize});
    }
    [HttpGet("maintenance")]
    public async Task<IActionResult> Maintenance([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]string? search=null,[FromQuery]string? status=null,CancellationToken ct=default)
    {
        Page(page,pageSize,search);var p=await access.GetProfile(Actor,ct);var q=AssetOperationsService.Scoped(db.MaintenanceOrders.AsNoTracking(),p,"resources.manage");
        if(!string.IsNullOrEmpty(search))q=q.Where(m=>m.Id.Contains(search)||m.Content.Contains(search)||db.AssetGroups.Any(a=>a.Id==m.AssetGroupId&&(a.Name.Contains(search)||a.Code.Contains(search))));if(!string.IsNullOrEmpty(status))q=q.Where(m=>m.Status==status);
        var total=await q.CountAsync(ct);var rows=await q.OrderByDescending(m=>m.Start).ThenBy(m=>m.Id).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);var ids=rows.Select(m=>m.AssetGroupId).ToArray();var names=await db.AssetGroups.Where(a=>ids.Contains(a.Id)).ToDictionaryAsync(a=>a.Id,a=>a.Name,ct);
        var views=await operations.FinancialViews(rows,ct);return Ok(new{items=views.Select(m=>new{maintenance=m,name=names.GetValueOrDefault(m.AssetGroupId,m.AssetGroupId)}),total,page,pageSize});
    }
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken ct)
    {
        var p=await access.GetProfile(Actor,ct);if(!new[]{"resources.manage","requests.review","logs.review","inventory.manage"}.Any(k=>Has(p,k)))throw AppException.Forbidden();
        int? roomCount=null,assetCount=null,owned=null,pending=null,returns=null,openInventory=null,runningMaintenance=null,pendingLogs=null;
        if(Has(p,"resources.manage")){var assets=AssetOperationsService.Scoped(db.AssetGroups.AsNoTracking(),p,"resources.manage");roomCount=await AssetOperationsService.Scoped(db.Rooms.AsNoTracking(),p,"resources.manage").CountAsync(ct);assetCount=await assets.CountAsync(ct);owned=await assets.SumAsync(a=>(int?)a.Total,ct)??0;runningMaintenance=await AssetOperationsService.Scoped(db.MaintenanceOrders.AsNoTracking(),p,"resources.manage").CountAsync(m=>m.Status=="running",ct);}
        if(Has(p,"requests.review"))pending=await RoomsFor(p,"requests.review").CountAsync(r=>r.Status=="pending",ct)+await LoansFor(p).CountAsync(r=>r.Status=="pending",ct);
        if(Has(p,"requests.review")||Has(p,"resources.manage"))returns=await LoansFor(p).CountAsync(r=>r.Status=="return_pending",ct);
        if(Has(p,"inventory.manage"))openInventory=await AssetOperationsService.Scoped(db.InventorySessions.AsNoTracking(),p,"inventory.manage").CountAsync(i=>i.Status=="open",ct);
        if(Has(p,"logs.review")){var roomIds=RoomsFor(p,"logs.review").Where(r=>r.Status=="approved").Select(r=>r.Id);var loanIds=LogsLoans(p).Select(r=>r.Id);pendingLogs=await db.RoomUseLogs.CountAsync(l=>roomIds.Contains(l.RequestId)&&l.Status=="submitted",ct)+await db.EquipmentUseLogs.CountAsync(l=>loanIds.Contains(l.LoanId)&&l.Status=="submitted",ct);}
        return Ok(new{roomCount,assetCount,owned,pending,returns,openInventory,runningMaintenance,pendingLogs,generatedAt=DateTimeOffset.UtcNow});
    }
}
