using System.Data;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class ReportService(HpuniDbContext db,IAccessService access)
{
    static void Year(int year){if(year is <2000 or >2100)throw AppException.Invalid("Năm báo cáo phải từ 2000 đến 2100.");}
    async Task<AccessProfile> Profile(string actor,string permission,string? department,CancellationToken ct)
    {
        var p=await access.GetProfile(actor,ct);
        if(!AccessRules.HasPermission(p,permission)||department!=null&&!AccessRules.Allows(p,permission,department,null))throw AppException.Forbidden();
        return p;
    }
    static IQueryable<T> Scope<T>(IQueryable<T> q,AccessProfile p,string permission,string? dep) where T:class
    {
        q=AssetOperationsService.Scoped(q.AsNoTracking(),p,permission);
        return dep==null?q:q.Where(x=>EF.Property<string>(x,"DepartmentId")==dep);
    }
    static IQueryable<T> FinanceScope<T>(IQueryable<T> q,AccessProfile p,string permission,string? dep) where T:class=>
        AssetOperationsService.Scoped(Scope(q,p,permission,dep),p,"budget.read");
    // Serializable gives one coherent view without requiring a database-wide SNAPSHOT option.
    // A deadlock victim can safely retry a read-only report in a fresh transaction.
    async Task<T> Consistent<T>(Func<Task<T>> read,CancellationToken ct)
    {
        for(var attempt=0;;attempt++){
            try{
                await using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
                var result=await read();await tx.CommitAsync(ct);return result;
            }catch(SqlException e) when(e.Number==1205&&attempt<2){await Task.Delay(50*(attempt+1),ct);}
        }
    }
    public async Task<FinanceReport> Finance(string actor,int year,string? department,string? category,bool export,CancellationToken ct)
    {
        Year(year);if(category!=null&&!FinanceService.Categories.Contains(category))throw AppException.Invalid("Khoản mục không hợp lệ.");
        var permission=export?"reports.export":"reports.read";var p=await Profile(actor,permission,department,ct);
        if(!AccessRules.HasPermission(p,"budget.read")||department!=null&&!AccessRules.Allows(p,"budget.read",department,null))throw AppException.Forbidden();
        return await Consistent(async()=>{
            var aq=FinanceScope(db.BudgetAllocations,p,permission,department).Where(x=>x.Year==year);
            var eq=FinanceScope(db.FinanceEntries,p,permission,department).Where(x=>x.Year==year&&!db.FinanceReversals.Any(r=>r.EntryId==x.Id));
            var jq=FinanceScope(db.BudgetAdjustments,p,permission,department).Where(x=>x.Year==year&&x.Status=="approved");
            if(category!=null){aq=aq.Where(x=>x.Category==category);eq=eq.Where(x=>x.Category==category);jq=jq.Where(x=>x.Category==category);}
            var allocations=await aq.Select(x=>new{x.DepartmentId,x.Category,x.Amount}).Concat(jq.Select(x=>new{x.DepartmentId,x.Category,x.Amount})).GroupBy(x=>new{x.DepartmentId,x.Category}).Select(g=>new{g.Key.DepartmentId,g.Key.Category,Amount=g.Sum(x=>x.Amount)}).Take(5001).ToArrayAsync(ct);
            var entries=await eq.GroupBy(x=>new{x.DepartmentId,x.Category}).Select(g=>new{g.Key.DepartmentId,g.Key.Category,
                Received=g.Sum(x=>x.Kind=="receipt"?x.Amount:0),Spent=g.Sum(x=>x.Kind=="expense"?x.Amount:0)}).Take(5001).ToArrayAsync(ct);
            var maintenance=await (from e in eq where e.Kind=="expense"
                join r in db.PaymentRequests on e.PaymentRequestId equals r.Id where r.MaintenanceId!=null
                group e by new{e.DepartmentId,e.Category} into g select new{g.Key.DepartmentId,g.Key.Category,Amount=g.Sum(e=>e.Amount)}).Take(5001).ToArrayAsync(ct);
            var requests=await FinanceScope(db.PaymentRequests,p,permission,department).Where(x=>x.Year==year&&(x.Status=="pending"||x.Status=="part_paid"))
                .GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Amount=g.Sum(x=>x.Amount-(db.FinanceEntries.Where(e=>e.PaymentRequestId==x.Id&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).Sum(e=>(decimal?)e.Amount)??0)),Maintenance=g.Sum(x=>x.MaintenanceId!=null?x.Amount-(db.FinanceEntries.Where(e=>e.PaymentRequestId==x.Id&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).Sum(e=>(decimal?)e.Amount)??0):0)}).Take(5001).ToArrayAsync(ct);
            var unqueued=await FinanceScope(db.MaintenanceOrders,p,permission,department).Where(x=>x.Status=="completed"&&x.Cost>0&&!db.PaymentRequests.Any(r=>r.MaintenanceId==x.Id))
                .GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Amount=g.Sum(x=>x.Cost)}).Take(5001).ToArrayAsync(ct);
            if(new[]{allocations.Length,entries.Length,maintenance.Length,requests.Length,unqueued.Length}.Any(n=>n>5000))throw AppException.Invalid("Báo cáo quá lớn; hãy lọc theo đơn vị.");
            var names=await db.Departments.AsNoTracking().ToDictionaryAsync(x=>x.Id,x=>x.Name,ct);
            string Name(string id)=>names.GetValueOrDefault(id,id);
            var keys=allocations.Select(x=>(x.DepartmentId,x.Category)).Union(entries.Select(x=>(x.DepartmentId,x.Category))).OrderBy(x=>x.DepartmentId).ThenBy(x=>x.Category);
            var rows=keys.Select(k=>new FinanceReportRow(k.DepartmentId,Name(k.DepartmentId),k.Category,
                allocations.FirstOrDefault(x=>x.DepartmentId==k.DepartmentId&&x.Category==k.Category)?.Amount??0,
                entries.FirstOrDefault(x=>x.DepartmentId==k.DepartmentId&&x.Category==k.Category)?.Received??0,
                entries.FirstOrDefault(x=>x.DepartmentId==k.DepartmentId&&x.Category==k.Category)?.Spent??0,
                maintenance.FirstOrDefault(x=>x.DepartmentId==k.DepartmentId&&x.Category==k.Category)?.Amount??0)).ToArray();
            var pending=requests.Select(x=>x.DepartmentId).Union(unqueued.Select(x=>x.DepartmentId)).Order().Select(id=>new PendingCostRow(id,Name(id),
                requests.FirstOrDefault(x=>x.DepartmentId==id)?.Amount??0,requests.FirstOrDefault(x=>x.DepartmentId==id)?.Maintenance??0,unqueued.FirstOrDefault(x=>x.DepartmentId==id)?.Amount??0)).ToArray();
            return new FinanceReport(year,category,DateTimeOffset.UtcNow,rows,pending);
        },ct);
    }
    public async Task<ResourceReport> Resources(string actor,int year,string? department,bool export,CancellationToken ct)
    {
        Year(year);var permission=export?"reports.export":"reports.read";var p=await Profile(actor,permission,department,ct);
        var start=new DateTimeOffset(year,1,1,0,0,0,TimeSpan.FromHours(7));var end=start.AddYears(1);var now=DateTimeOffset.UtcNow;
        return await Consistent(async()=>{
            var assetQuery=Scope(db.AssetGroups,p,permission,department);
            var roomQuery=Scope(db.Rooms,p,permission,department);
            var assets=await assetQuery.GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Count=g.LongCount(),Owned=g.Sum(x=>(long)x.Total),Damaged=g.Sum(x=>(long)x.Damaged),Lost=g.Sum(x=>(long)x.Lost)}).Take(5001).ToArrayAsync(ct);
            var acquisition=await (from a in assetQuery join h in db.FixedAssetProfiles.AsNoTracking() on a.Id equals h.AssetGroupId
                group h by a.DepartmentId into g select new{DepartmentId=g.Key,Count=g.LongCount(),Value=g.Sum(x=>x.AcquisitionValue)}).Take(5001).ToArrayAsync(ct);
            var loanQuery=from l in db.EquipmentLoans.AsNoTracking() join a in assetQuery on l.AssetGroupId equals a.Id
                select new{DepartmentId=a.DepartmentId,l.Start,l.End,l.Status,l.Quantity,l.Returned};
            var loans=await loanQuery.GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,
                InYear=g.LongCount(x=>x.Start>=start&&x.Start<end),
                Outstanding=g.Sum(x=>x.Status=="ready"||x.Status=="receipt_disputed"||x.Status=="borrowed"||x.Status=="return_pending"?(long)x.Quantity-x.Returned:0),
                Overdue=g.LongCount(x=>(x.Status=="ready"||x.Status=="receipt_disputed"||x.Status=="borrowed"||x.Status=="return_pending")&&x.End<now&&x.Quantity>x.Returned)}).Take(5001).ToArrayAsync(ct);
            var jobs=await Scope(db.MaintenanceOrders,p,permission,department).Where(x=>x.Status=="planned"||x.Status=="running").GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Planned=g.LongCount(x=>x.Status=="planned"),Running=g.LongCount(x=>x.Status=="running")}).Take(5001).ToArrayAsync(ct);
            var rooms=await roomQuery.Where(x=>x.Active).GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Count=g.LongCount()}).Take(5001).ToArrayAsync(ct);
            var bookingQuery=from b in db.RoomRequests.AsNoTracking() join r in roomQuery on b.RoomId equals r.Id select new{DepartmentId=r.DepartmentId,b.Start,b.Status};
            var bookings=await bookingQuery.Where(x=>x.Start>=start&&x.Start<end).GroupBy(x=>x.DepartmentId).Select(g=>new{DepartmentId=g.Key,Count=g.LongCount(),Approved=g.LongCount(x=>x.Status=="approved")}).Take(5001).ToArrayAsync(ct);
            if(new[]{assets.Length,acquisition.Length,loans.Length,jobs.Length,rooms.Length,bookings.Length}.Any(n=>n>5000))throw AppException.Invalid("Báo cáo quá lớn; hãy lọc theo đơn vị.");
            var names=await db.Departments.AsNoTracking().ToDictionaryAsync(x=>x.Id,x=>x.Name,ct);
            var ids=assets.Select(x=>x.DepartmentId).Union(loans.Select(x=>x.DepartmentId)).Union(jobs.Select(x=>x.DepartmentId)).Union(rooms.Select(x=>x.DepartmentId)).Union(bookings.Select(x=>x.DepartmentId));
            return new ResourceReport(year,now,ids.Order().Select(id=>{
                var a=assets.FirstOrDefault(x=>x.DepartmentId==id);var h=acquisition.FirstOrDefault(x=>x.DepartmentId==id);var l=loans.FirstOrDefault(x=>x.DepartmentId==id);var m=jobs.FirstOrDefault(x=>x.DepartmentId==id);var r=rooms.FirstOrDefault(x=>x.DepartmentId==id);var b=bookings.FirstOrDefault(x=>x.DepartmentId==id);
                return new ResourceReportRow(id,names.GetValueOrDefault(id,id)){AssetGroups=a?.Count??0,AcquisitionProfiles=h?.Count??0,HistoricalDeclaredValue=a!=null&&a.Count>0&&h?.Count==a.Count?h.Value:null,Owned=a?.Owned??0,Good=(a?.Owned??0)-(a?.Damaged??0)-(a?.Lost??0),Damaged=a?.Damaged??0,Lost=a?.Lost??0,Outstanding=l?.Outstanding??0,OverdueLoans=l?.Overdue??0,LoansStartingInYear=l?.InYear??0,MaintenancePlanned=m?.Planned??0,MaintenanceRunning=m?.Running??0,ActiveRooms=r?.Count??0,RoomRequestsStartingInYear=b?.Count??0,ApprovedRoomRequestsStartingInYear=b?.Approved??0};
            }).ToArray());
        },ct);
    }
    public static byte[] FinanceCsv(FinanceReport r)
    {
        List<object?[]> rows=[["Loại dòng","Năm","Đơn vị","Khoản mục","Được cấp","Thu","Đã chi","Khả dụng","Chi bảo trì đã ghi","Chờ chi (chưa gán khoản mục)","Bảo trì chờ chi","Bảo trì chưa chuyển hàng chờ (mọi năm)"]];
        rows.AddRange(r.Rows.Select(x=>new object?[]{"Ngân sách",r.Year,x.DepartmentName,x.Category,x.Allocated,x.Received,x.Spent,x.Available,x.MaintenanceSpent,null,null,null}));
        rows.AddRange(r.PendingCosts.Select(x=>new object?[]{"Hàng chờ",r.Year,x.DepartmentName,null,null,null,null,null,null,x.Pending,x.MaintenancePending,x.UnqueuedMaintenanceAllTime}));
        return CsvDocument.Create(rows);
    }
    public static byte[] ResourceCsv(ResourceReport r)=>CsvDocument.Create(new object?[][]{["Năm lượt sử dụng","Đơn vị","Nhóm tài sản hiện tại","Sở hữu hiện tại","Tốt (gồm đang mượn)","Hỏng","Mất","Đang bàn giao/mượn","Phiếu quá hạn","Lượt mượn bắt đầu trong năm","Bảo trì đã lên lịch","Bảo trì đang chạy","Phòng hoạt động","Lượt đặt phòng bắt đầu trong năm","Lượt phòng đã duyệt","Giá trị còn lại (chưa xác định)","Thời điểm","Nhóm đã kê khai nguyên giá","Nhóm thiếu kê khai nguyên giá","Nguyên giá lịch sử đủ kê khai (không phải giá trị còn lại)"]}.Concat(r.Rows.Select(x=>new object?[]{r.Year,x.DepartmentName,x.AssetGroups,x.Owned,x.Good,x.Damaged,x.Lost,x.Outstanding,x.OverdueLoans,x.LoansStartingInYear,x.MaintenancePlanned,x.MaintenanceRunning,x.ActiveRooms,x.RoomRequestsStartingInYear,x.ApprovedRoomRequestsStartingInYear,x.PurchaseValue,r.GeneratedAt.ToString("O"),x.AcquisitionProfiles,x.MissingAcquisitionProfiles,x.HistoricalDeclaredValue})));
}
