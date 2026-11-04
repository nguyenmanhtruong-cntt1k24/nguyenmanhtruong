using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class AssetOperationsService(HpuniDbContext db, IAccessService access, AdminService admin,
    EquipmentService equipment, IdempotentCommands commands)
{
    private async Task Demand(string actor,string permission,string department,CancellationToken ct)
    {
        if(!AccessRules.Allows(await access.GetProfile(actor,ct),permission,department,null)) throw AppException.Forbidden();
    }
    public static IQueryable<T> Scoped<T>(IQueryable<T> query,AccessProfile p,string permission) where T:class
    {
        if(!AccessRules.HasPermission(p,permission))throw AppException.Forbidden();
        var deps=p.DepartmentIds.ToArray();
        var all=p.Grants.Any(g=>g.Permissions.Contains(permission)&&g.Scope=="all");
        var unit=p.Grants.Any(g=>g.Permissions.Contains(permission)&&g.Scope=="unit");
        return query.Where(x=>(p.Scope=="all"||p.Scope=="unit"&&EF.Property<string>(x,"DepartmentId")==p.DepartmentId||p.Scope=="units"&&deps.Contains(EF.Property<string>(x,"DepartmentId")))
            &&(all||unit&&EF.Property<string>(x,"DepartmentId")==p.DepartmentId));
    }
    public static MaintenanceView View(MaintenanceOrder m)=>new(m.Id,m.AssetGroupId,m.DepartmentId,m.Mode,m.Quantity,m.Start,m.End,m.Status,m.Content,m.Result,m.Note,m.Cost,Convert.ToBase64String(m.RowVersion));
    public async Task<MaintenanceView> FinancialView(MaintenanceOrder m,CancellationToken ct)
    {
        return (await FinancialViews([m],ct))[0];
    }
    public async Task<MaintenanceView[]> FinancialViews(MaintenanceOrder[] rows,CancellationToken ct)
    {
        var ids=rows.Select(m=>m.Id).ToArray();
        var payments=await db.PaymentRequests.AsNoTracking().Where(p=>ids.Contains(p.MaintenanceId!)).ToArrayAsync(ct);
        var paymentIds=payments.Select(p=>p.Id).ToArray();
        var entries=await db.FinanceEntries.AsNoTracking().Where(e=>paymentIds.Contains(e.PaymentRequestId!)&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).ToArrayAsync(ct);
        return rows.Select(m=>{
            var p=payments.SingleOrDefault(p=>p.MaintenanceId==m.Id);var e=entries.Where(e=>p!=null&&e.PaymentRequestId==p.Id).OrderByDescending(e=>e.CreatedAt).ThenBy(e=>e.Id).FirstOrDefault();
            return View(m) with{PaymentRequestId=p?.Id,PaymentStatus=p?.Status,Voucher=e?.Voucher,FinanceEntryId=e?.Id};
        }).ToArray();
    }
    public async Task<MaintenanceOrder> GetMaintenance(string actor,string id,CancellationToken ct)
    {
        var m=await db.MaintenanceOrders.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await Demand(actor,"resources.manage",m.DepartmentId,ct);return m;
    }
    public Task<MaintenanceView> Plan(string actor,string key,PlanMaintenance input,CancellationToken ct)=>commands.Run(actor,key,"maintenance.plan",input,async()=>{
        var a=await db.AssetGroups.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==input.AssetGroupId,ct)??throw AppException.Missing();
        await Demand(actor,"resources.manage",a.DepartmentId,ct);
    },async()=>{
        await TransactionLocks.Acquire(db,"asset:"+input.AssetGroupId,ct);
        var a=await db.AssetGroups.SingleAsync(x=>x.Id==input.AssetGroupId,ct);
        await Demand(actor,"resources.manage",a.DepartmentId,ct);
        if(!a.Active||input.Mode is not("service" or "repair")||input.Quantity is <1 or >1000000||input.Start>=input.End||input.End<=DateTimeOffset.UtcNow||input.End-input.Start>TimeSpan.FromDays(90))throw AppException.Invalid("Loại, số lượng hoặc lịch bảo trì không hợp lệ (tối đa 90 ngày).");
        // One live order per group keeps the damaged pool unambiguous without serial tracking.
        if(await db.MaintenanceOrders.AnyAsync(x=>x.AssetGroupId==a.Id&&(x.Status=="planned"||x.Status=="running"),ct))throw new AppException(409,"MAINTENANCE_ACTIVE","Nhóm tài sản đã có kế hoạch chưa hoàn tất.");
        if(input.Mode=="repair"?input.Quantity>a.Damaged:await equipment.Free(a,input.Start,input.End,null,ct)<input.Quantity)throw new AppException(409,"INSUFFICIENT_EQUIPMENT","Không đủ phần hỏng hoặc thiết bị tốt trong lịch bảo trì.");
        var m=new MaintenanceOrder{AssetGroupId=a.Id,DepartmentId=a.DepartmentId,Mode=input.Mode,Quantity=input.Quantity,Start=input.Start.ToUniversalTime(),End=input.End.ToUniversalTime(),Content=AdminService.OptionalNote(input.Content)};
        db.MaintenanceOrders.Add(m);admin.Audit(actor,"maintenance.plan",m.Id,input);
        await db.SaveChangesAsync(ct);return View(m);
    },ct);
    public Task<MaintenanceView> MaintenanceAction(string actor,string id,string operation,string key,FinishMaintenance input,CancellationToken ct)=>commands.Run(actor,key,"maintenance."+operation+":"+id,input,
        async()=>{await GetMaintenance(actor,id,ct);},async()=>{
            var snapshot=await GetMaintenance(actor,id,ct);await TransactionLocks.Acquire(db,"asset:"+snapshot.AssetGroupId,ct);
            var m=await db.MaintenanceOrders.SingleAsync(x=>x.Id==id,ct);var a=await db.AssetGroups.SingleAsync(x=>x.Id==m.AssetGroupId,ct);
            await Demand(actor,"resources.manage",m.DepartmentId,ct);AdminService.Version(m.RowVersion,input.Version);
            if(operation=="cancel") {if(m.Status!="planned")throw AppException.Invalid("Chỉ hủy kế hoạch chưa bắt đầu.");m.Status="cancelled";}
            else if(operation=="start") {
                if(m.Status!="planned"||DateTimeOffset.UtcNow<m.Start||DateTimeOffset.UtcNow>=m.End)throw AppException.Invalid("Chưa đến lịch hoặc kế hoạch đã hết thời gian bắt đầu.");
                if(m.Mode=="repair"?m.Quantity>a.Damaged:await equipment.Free(a,DateTimeOffset.UtcNow,m.End,null,ct,m.Id)<m.Quantity)throw new AppException(409,"INSUFFICIENT_EQUIPMENT","Thiết bị thực tế không đủ để bắt đầu.");
                m.Status="running";
            } else if(operation=="finish") {
                if(m.Status!="running"||input.Result is not("good" or "unavailable")||input.Cost<0||input.Cost>1000000000000m||decimal.Truncate(input.Cost)!=input.Cost)throw AppException.Invalid("Chỉ hoàn tất công việc đang chạy; kết quả và chi phí nguyên đồng phải hợp lệ.");
                if(m.Mode=="repair"&&input.Result=="good") {
                    if(a.Damaged<m.Quantity)throw AppException.Invalid("Số hỏng hiện tại không đủ để phục hồi.");a.Damaged-=m.Quantity;
                    Move(actor,a.Id,"repair.restored",m.Quantity,input.Note);
                } else if(m.Mode=="service"&&input.Result=="unavailable") {a.Damaged+=m.Quantity;Move(actor,a.Id,"service.damaged",m.Quantity,input.Note);}
                m.Status="completed";m.Result=input.Result;m.Cost=input.Cost;
                await FinanceService.QueueMaintenance(db,m,ct);
            } else throw AppException.Invalid("Thao tác không hợp lệ.");
            m.Note=AdminService.OptionalNote(input.Note);db.Entry(a).Property(x=>x.Total).IsModified=true;
            admin.Audit(actor,"maintenance."+operation,id,input);await db.SaveChangesAsync(ct);return View(m);
        },ct);
    private void Move(string actor,string asset,string kind,int quantity,string? note)=>db.EquipmentMovements.Add(new(){ActorId=actor,AssetGroupId=asset,Kind=kind,Quantity=quantity,Note=AdminService.OptionalNote(note)});

    public Task<InventoryView> CreateInventory(string actor,string key,CreateInventory input,CancellationToken ct)=>commands.Run(actor,key,"inventory.create",input,
        ()=>Demand(actor,"inventory.manage",input.DepartmentId,ct),async()=>{
            await TransactionLocks.Acquire(db,"inventory-catalog:"+input.DepartmentId,ct);
            await TransactionLocks.Acquire(db,"equipment-catalog",ct);
            var rows=await db.AssetGroups.AsNoTracking().Where(x=>x.DepartmentId==input.DepartmentId).OrderBy(x=>x.Id).ToArrayAsync(ct);
            if(rows.Length==0||rows.Length>500)throw AppException.Invalid("Đơn vị cần có từ 1 đến 500 nhóm tài sản để lập đợt.");
            foreach(var a in rows)await TransactionLocks.Acquire(db,"asset:"+a.Id,ct);
            rows=await db.AssetGroups.AsNoTracking().Where(x=>x.DepartmentId==input.DepartmentId).OrderBy(x=>x.Id).ToArrayAsync(ct);
            var s=new InventorySession{Name=AdminService.Required(input.Name,150),DepartmentId=input.DepartmentId};
            db.InventorySessions.Add(s);
            db.InventoryItems.AddRange(rows.Select(a=>new InventoryItem{SessionId=s.Id,AssetGroupId=a.Id,Code=a.Code,Name=a.Name,Expected=a.Total,ExpectedDamaged=a.Damaged,ExpectedLost=a.Lost,AssetVersion=Convert.ToBase64String(a.RowVersion)}));
            admin.Audit(actor,"inventory.create",s.Id,new{input,items=rows.Length});await db.SaveChangesAsync(ct);return await InventoryView(s,ct);
        },ct);
    public async Task<InventorySession> GetInventory(string actor,string id,CancellationToken ct)
    {
        var s=await db.InventorySessions.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await Demand(actor,"inventory.manage",s.DepartmentId,ct);return s;
    }
    public async Task<InventoryView> InventoryView(InventorySession s,CancellationToken ct)=>new(s.Id,s.Name,s.DepartmentId,s.Status,s.At,Convert.ToBase64String(s.RowVersion),
        await db.InventoryItems.AsNoTracking().Where(x=>x.SessionId==s.Id).OrderBy(x=>x.Code).Select(x=>new InventoryItemView(x.AssetGroupId,x.Code,x.Name,x.Expected,x.ExpectedDamaged,x.ExpectedLost,x.AssetVersion,x.Actual,x.Condition,x.Note)).ToArrayAsync(ct));
    public Task<InventoryView> SaveInventory(string actor,string id,string key,SaveInventory input,CancellationToken ct)=>commands.Run(actor,key,"inventory.save:"+id,input,
        async()=>{await GetInventory(actor,id,ct);},async()=>{
            await TransactionLocks.Acquire(db,"inventory:"+id,ct);var s=await db.InventorySessions.SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
            await Demand(actor,"inventory.manage",s.DepartmentId,ct);AdminService.Version(s.RowVersion,input.Version);
            if(s.Status!="open")throw AppException.Invalid("Đợt kiểm kê đã hoàn tất.");
            var rows=await db.InventoryItems.Where(x=>x.SessionId==id).ToArrayAsync(ct);
            if(input.Items==null||input.Items.Length!=rows.Length||input.Items.Select(x=>x.AssetGroupId).Distinct().Count()!=rows.Length||input.Items.Any(x=>!rows.Any(r=>r.AssetGroupId==x.AssetGroupId)))throw AppException.Invalid("Phải gửi đủ, không trùng các dòng trong ảnh chụp kiểm kê.");
            foreach(var row in rows){var item=input.Items.Single(x=>x.AssetGroupId==row.AssetGroupId);
                if(item.Actual is <0 or >1000000||item.Condition is not("" or "good" or "damaged" or "missing")||input.Complete&&(item.Actual==null||item.Condition==""))throw AppException.Invalid("Số kiểm đếm hoặc tình trạng chưa hợp lệ/đầy đủ.");
                row.Actual=item.Actual;row.Condition=item.Condition;row.Note=AdminService.OptionalNote(item.Note);}
            if(input.Complete)s.Status="completed";db.Entry(s).Property(x=>x.Name).IsModified=true;
            admin.Audit(actor,"inventory.save",id,input);await db.SaveChangesAsync(ct);return await InventoryView(s,ct);
        },ct);

    private async Task<EquipmentLoan> DemandLog(string actor,string id,bool review,CancellationToken ct)
    {
        var r=await db.EquipmentLoans.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        var p=await access.GetProfile(actor,ct);
        if(!AccessRules.Allows(p,review?"logs.review":"logs.write",r.DepartmentId,r.OwnerId)||!review&&r.OwnerId!=actor)throw AppException.Missing();
        if(r.Status is not("borrowed" or "return_pending" or "completed"))throw AppException.Invalid("Cần xác nhận nhận thiết bị trước khi ghi nhật ký.");
        return r;
    }
    public static EquipmentLogView View(EquipmentUseLog l)=>new(l.Id,l.LoanId,l.Used,l.ActualStart,l.ActualEnd,l.Quantity,l.Condition,l.Issue,l.Status,l.ReviewNote,Convert.ToBase64String(l.RowVersion));
    public async Task<EquipmentLogView> GetLog(string actor,string loan,bool review,CancellationToken ct)
    {
        await DemandLog(actor,loan,review,ct);return View(await db.EquipmentUseLogs.AsNoTracking().SingleOrDefaultAsync(x=>x.LoanId==loan,ct)??throw AppException.Missing());
    }
    public Task<EquipmentLogView> SaveLog(string actor,string loan,string key,SaveEquipmentLog input,CancellationToken ct)=>commands.Run(actor,key,"equipment-log.save:"+loan,input,
        async()=>{await DemandLog(actor,loan,false,ct);},async()=>{
            await TransactionLocks.Acquire(db,"equipment-log:"+loan,ct);var r=await DemandLog(actor,loan,false,ct);
            if(r.End>DateTimeOffset.UtcNow)throw AppException.Invalid("Chỉ gửi nhật ký sau khi kết thúc lịch mượn.");
            var l=await db.EquipmentUseLogs.SingleOrDefaultAsync(x=>x.LoanId==loan,ct);
            if(l!=null){AdminService.Version(l.RowVersion,input.Version);if(l.Status!="needs_changes")throw AppException.Invalid("Chỉ sửa nhật ký khi được yêu cầu bổ sung.");}
            else {if(input.Version!=null)throw AppException.Stale();l=new(){LoanId=loan};db.EquipmentUseLogs.Add(l);}
            if(input.Used?(input.ActualStart==null||input.ActualEnd==null||input.Quantity==null||input.Quantity<1||input.Quantity>r.Quantity||input.ActualStart>=input.ActualEnd||input.ActualEnd>DateTimeOffset.UtcNow||input.ActualEnd-input.ActualStart>TimeSpan.FromDays(30)||input.ActualStart<r.HandedAt):input.ActualStart!=null||input.ActualEnd!=null||input.Quantity!=null)throw AppException.Invalid("Thời gian hoặc số lượng thực tế không hợp lệ.");
            l.Used=input.Used;l.ActualStart=input.ActualStart?.ToUniversalTime();l.ActualEnd=input.ActualEnd?.ToUniversalTime();l.Quantity=input.Quantity;l.Condition=AdminService.OptionalNote(input.Condition);l.Issue=AdminService.OptionalNote(input.Issue);l.Status="submitted";
            admin.Audit(actor,"equipment-log.save",loan,input);await db.SaveChangesAsync(ct);return View(l);
        },ct);
    public Task<EquipmentLogView> ReviewLog(string actor,string loan,string key,ReviewRoomLog input,CancellationToken ct)=>commands.Run(actor,key,"equipment-log.review:"+loan,input,
        async()=>{await DemandLog(actor,loan,true,ct);},async()=>{
            await TransactionLocks.Acquire(db,"equipment-log:"+loan,ct);await DemandLog(actor,loan,true,ct);
            var l=await db.EquipmentUseLogs.SingleOrDefaultAsync(x=>x.LoanId==loan,ct)??throw AppException.Missing();AdminService.Version(l.RowVersion,input.Version);
            if(l.Status!="submitted"||input.Result is not("confirmed" or "needs_changes"))throw AppException.Invalid("Kết quả hoặc trạng thái nhật ký không hợp lệ.");
            l.Status=input.Result;l.ReviewNote=AdminService.OptionalNote(input.Note);admin.Audit(actor,"equipment-log.review",loan,input);await db.SaveChangesAsync(ct);return View(l);
        },ct);
}
