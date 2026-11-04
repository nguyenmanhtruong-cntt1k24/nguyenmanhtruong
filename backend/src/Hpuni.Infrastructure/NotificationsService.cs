using System.Security.Cryptography;
using System.Text;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class NotificationsService(HpuniDbContext db,IAccessService access,EquipmentService equipment)
{
    // Every branch is scoped before SQL count/pagination; no cached recipients or source bodies.
    sealed class Candidate
    {
        public string Key { get; set; }="";
        public string Kind { get; set; }="";
        public string SourceId { get; set; }="";
        public string DepartmentId { get; set; }="";
        public string OwnerId { get; set; }="";
        public byte[] Version { get; set; }=[];
        public string Title { get; set; }="";
        public string Message { get; set; }="";
        public DateTimeOffset At { get; set; }
        public string Severity { get; set; }="info";
        public string Url { get; set; }="";
    }
    static IQueryable<Candidate> Scope(IQueryable<Candidate> q,AccessProfile p,string permission)
    {
        var has=AccessRules.HasPermission(p,permission);var deps=p.DepartmentIds.ToArray();
        var all=p.Grants.Any(g=>g.Permissions.Contains(permission)&&g.Scope=="all");
        var unit=p.Grants.Any(g=>g.Permissions.Contains(permission)&&g.Scope=="unit");
        var self=p.Grants.Any(g=>g.Permissions.Contains(permission)&&g.Scope=="self");
        return q.Where(x=>has&&(p.Scope=="all"||p.Scope=="unit"&&x.DepartmentId==p.DepartmentId||p.Scope=="units"&&deps.Contains(x.DepartmentId)||p.Scope=="self"&&x.OwnerId==p.UserId)
            &&(all||unit&&x.DepartmentId==p.DepartmentId||self&&x.OwnerId==p.UserId));
    }
    IQueryable<Candidate> Query(AccessProfile p,DateTimeOffset now)
    {
        var soon=now.AddHours(48);var week=now.AddDays(7);var recent=now.AddDays(-30);
        var rooms=Scope(db.RoomRequests.AsNoTracking().Where(r=>r.Status=="pending").Select(r=>new Candidate{
            Key="room-review:"+r.Id,Kind="room-review",SourceId=r.Id,DepartmentId=r.DepartmentId,OwnerId=r.OwnerId,Version=r.RowVersion,
            Title="Đăng ký phòng chờ duyệt",Message=r.Purpose,At=r.CreatedAt,Severity="info",Url="resources.html#/approvals"}),p,"requests.review");
        var ownRooms=Scope(db.RoomRequests.AsNoTracking().Where(r=>r.OwnerId==p.UserId&&
            (r.Status=="approved"&&r.End>now&&r.Start<=week||r.Status=="rejected"&&r.CreatedAt>=recent)).Select(r=>new Candidate{
            Key="room-"+r.Status+":"+r.Id,Kind="room-"+r.Status,SourceId=r.Id,DepartmentId=r.DepartmentId,OwnerId=r.OwnerId,Version=r.RowVersion,
            Title=r.Status=="approved"?"Lịch phòng đã được duyệt":"Đăng ký phòng bị từ chối",Message=r.ReviewNote,At=r.Start,Severity="info",Url="index.html#/requests"}),p,"rooms.book");
        var loans=equipment.Visible(p,true).Where(r=>r.Status=="pending"||r.Status=="return_pending"||r.Status=="receipt_disputed"||
            (r.Status=="ready"||r.Status=="borrowed")&&r.Quantity>r.Returned&&r.End<=soon).Select(r=>new Candidate{
                Key="loan-task-"+(r.End<now&&(r.Status=="ready"||r.Status=="borrowed")?"overdue":r.Status)+":"+r.Id,
                Kind="loan-task",SourceId=r.Id,DepartmentId=r.Asset.DepartmentId,OwnerId=r.OwnerId,Version=r.RowVersion,
                Title=r.Status=="pending"?"Đăng ký thiết bị chờ duyệt":r.Status=="return_pending"?"Yêu cầu trả chờ tiếp nhận":r.Status=="receipt_disputed"?"Bàn giao có sai lệch":r.End<now?"Thiết bị đã quá hạn trả":"Thiết bị sắp đến hạn trả",
                Message=r.Asset.Name,At=r.End,Severity=r.End<now&&r.Quantity>r.Returned&&r.Status!="pending"?"warning":"info",
                Url=r.Status=="pending"?"resources.html#/approvals":"resources.html#/loans"});
        var ownLoans=equipment.Visible(p,false).Where(r=>r.Status=="approved"&&r.End>now&&r.Start<=week||r.Status=="rejected"&&r.CreatedAt>=recent||
            (r.Status=="ready"||r.Status=="borrowed"||r.Status=="receipt_disputed"||r.Status=="return_pending")&&r.Quantity>r.Returned&&r.End<=soon).Select(r=>new Candidate{
                Key="loan-own-"+(r.End<now&&r.Status!="rejected"?"overdue":r.Status)+":"+r.Id,Kind="loan-own",SourceId=r.Id,
                DepartmentId=r.DepartmentId,OwnerId=r.OwnerId,Version=r.RowVersion,
                Title=r.Status=="approved"?"Đăng ký thiết bị đã được duyệt":r.Status=="rejected"?"Đăng ký thiết bị bị từ chối":r.End<now?"Bạn còn thiết bị quá hạn trả":"Thiết bị của bạn sắp đến hạn trả",
                Message=r.Asset.Name,At=r.End,Severity=r.End<now&&r.Status!="rejected"?"warning":"info",Url="index.html#/requests"});
        var maintenance=Scope(db.MaintenanceOrders.AsNoTracking().Where(m=>m.Status=="planned"&&m.Start<=week||m.Status=="running"&&m.End<=soon).Select(m=>new Candidate{
            Key="maintenance-"+(m.End<now?"overdue":m.Status)+":"+m.Id,Kind="maintenance",SourceId=m.Id,DepartmentId=m.DepartmentId,OwnerId="",Version=m.RowVersion,
            Title=m.Status=="planned"?(m.End<now?"Kế hoạch bảo trì đã hết lịch bắt đầu":"Bảo trì sắp đến lịch"):(m.End<now?"Bảo trì đang chạy quá hạn":"Bảo trì sắp đến hạn hoàn tất"),
            Message=m.Content,At=m.Status=="planned"?m.Start:m.End,Severity=m.End<now?"warning":"info",Url="resources.html#/maintenance"}),p,"resources.manage");
        var proposals=Scope(Scope(db.BudgetProposals.AsNoTracking().Where(x=>x.Status=="pending").Select(x=>new Candidate{
            Key="budget-review:"+x.Id,Kind="budget-review",SourceId=x.Id,DepartmentId=x.DepartmentId,OwnerId=x.OwnerId,Version=x.RowVersion,
            Title="Dự toán chờ phê duyệt",Message=x.Title,At=x.CreatedAt,Severity="info",Url="executive.html#/estimates"}),p,"budget.approve"),p,"budget.read");
        var adjustments=Scope(Scope(db.BudgetAdjustments.AsNoTracking().Where(x=>x.Status=="pending").Select(x=>new Candidate{
            Key="budget-adjustment:"+x.Id,Kind="budget-adjustment",SourceId=x.Id,DepartmentId=x.DepartmentId,OwnerId=x.ActorId,Version=x.RowVersion,
            Title="Điều chỉnh ngân sách chờ duyệt",Message=x.Reason,At=x.CreatedAt,Severity="info",Url="executive.html#/controls"}),p,"budget.approve"),p,"budget.read");
        var payments=Scope(Scope(db.PaymentRequests.AsNoTracking().Where(x=>x.Status=="pending"||x.Status=="part_paid").Select(x=>new Candidate{
            Key="payment-pending:"+x.Id,Kind="payment-pending",SourceId=x.Id,DepartmentId=x.DepartmentId,OwnerId="",Version=x.RowVersion,
            Title="Yêu cầu chi chờ thanh toán",Message=x.Content,At=x.CreatedAt,Severity="info",Url="accounting.html#/queue"}),p,"transactions.write"),p,"budget.read");
        var disposals=Scope(db.AssetDisposals.AsNoTracking().Where(x=>x.Status=="pending").Select(x=>new Candidate{
            Key="asset-disposal:"+x.Id,Kind="asset-disposal",SourceId=x.Id,DepartmentId=x.DepartmentId,OwnerId=x.ActorId,Version=x.RowVersion,
            Title="Đề nghị thanh lý tài sản chờ duyệt",Message=x.Reference+": "+x.Reason,At=x.CreatedAt,Severity="info",Url="executive.html?assetLifecycleId="+x.AssetGroupId+"#/lifecycle"}),p,"budget.approve");
        return rooms.Concat(ownRooms).Concat(loans).Concat(ownLoans).Concat(maintenance).Concat(proposals).Concat(adjustments).Concat(payments).Concat(disposals);
    }
    static string Id(Candidate c)=>Convert.ToBase64String(Encoding.UTF8.GetBytes(c.Key)).TrimEnd('=').Replace('+','-').Replace('/','_')+"."+Convert.ToHexString(c.Version);
    static (string Key,byte[] Version) Parse(string id)
    {
        try{
            if(id.Length>220)throw new FormatException();var parts=id.Split('.');
            if(parts.Length!=2||parts[1].Length!=16)throw new FormatException();
            var text=parts[0].Replace('-','+').Replace('_','/');text=text.PadRight((text.Length+3)/4*4,'=');
            var key=Encoding.UTF8.GetString(Convert.FromBase64String(text));
            if(key.Length>120||key.Any(char.IsControl))throw new FormatException();
            return(key,Convert.FromHexString(parts[1]));
        }catch(FormatException){throw AppException.Invalid("Mã thông báo không hợp lệ.");}
    }
    public async Task<NotificationPage> List(string actor,int page,int pageSize,bool unreadOnly,CancellationToken ct)
    {
        if(page is <1 or >100000||pageSize is <1 or >100)throw AppException.Invalid("Phân trang không hợp lệ.");
        var p=await access.GetProfile(actor,ct);if(!p.Active)throw AppException.Forbidden();
        var now=DateTimeOffset.UtcNow;var query=Query(p,now);
        var unread=query.Where(x=>!db.Set<NotificationReadState>().Any(r=>r.ActorId==actor&&r.SourceKey==x.Key&&r.SourceVersion==x.Version));
        var unreadCount=await unread.CountAsync(ct);if(unreadOnly)query=unread;
        var total=await query.CountAsync(ct);
        var rows=await query.OrderBy(x=>x.At).ThenBy(x=>x.Key).Skip((page-1)*pageSize).Take(pageSize).ToArrayAsync(ct);
        var keys=rows.Select(x=>x.Key).ToArray();var reads=await db.Set<NotificationReadState>().AsNoTracking().Where(x=>x.ActorId==actor&&keys.Contains(x.SourceKey)).ToArrayAsync(ct);
        return new(rows.Select(c=>new NotificationView(Id(c),c.Kind,c.SourceId,c.Title,c.Message,c.At,c.Severity,c.Url,
            reads.Any(r=>r.SourceKey==c.Key&&r.SourceVersion.SequenceEqual(c.Version)))).ToArray(),total,unreadCount,page,pageSize,now);
    }
    public async Task Read(string actor,string id,CancellationToken ct)
    {
        var parsed=Parse(id);var p=await access.GetProfile(actor,ct);if(!p.Active)throw AppException.Forbidden();
        await using var tx=await db.Database.BeginTransactionAsync(ct);
        var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(actor+":"+parsed.Key)));
        await TransactionLocks.Acquire(db,"notification-read:"+hash,ct);
        // A stale ID or a removed permission cannot acknowledge (or reveal) a source.
        if(!await Query(p,DateTimeOffset.UtcNow).AnyAsync(x=>x.Key==parsed.Key&&x.Version==parsed.Version,ct))throw AppException.Missing();
        var state=await db.Set<NotificationReadState>().SingleOrDefaultAsync(x=>x.ActorId==actor&&x.SourceKey==parsed.Key,ct);
        if(state==null)db.Set<NotificationReadState>().Add(new(){ActorId=actor,SourceKey=parsed.Key,SourceVersion=parsed.Version});
        else{state.SourceVersion=parsed.Version;state.ReadAt=DateTimeOffset.UtcNow;}
        await db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
    }
}
