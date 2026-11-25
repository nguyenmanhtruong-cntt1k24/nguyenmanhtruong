using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class FinanceService(HpuniDbContext db, IAccessService access, AdminService admin, IdempotentCommands commands)
{
    public static readonly string[] Categories = ["general", "maintenance", "equipment", "teaching", "operations"];
    public async Task Demand(string actor,string permission,string department,CancellationToken ct)
    {
        if(!AccessRules.Allows(await access.GetProfile(actor,ct),permission,department,null))throw AppException.Forbidden();
    }
    static void Year(int year){if(year is <2000 or >2100)throw AppException.Invalid("Năm tài chính phải từ 2000 đến 2100.");}
    static void Category(string category){if(!Categories.Contains(category))throw AppException.Invalid("Khoản mục ngân sách không hợp lệ.");}
    static void Money(decimal amount){if(amount<=0||amount>1000000000000m||decimal.Truncate(amount)!=amount)throw AppException.Invalid("Số tiền phải là số nguyên đồng dương, tối đa 1.000 tỷ.");}
    async Task<string> Department(string id,CancellationToken ct,bool active=true)=>await db.Departments.Where(x=>x.Id==id&&(!active||x.Active)).Select(x=>x.Id).SingleOrDefaultAsync(ct)??throw AppException.Invalid("Đơn vị không tồn tại hoặc không hoạt động.");
    static string BudgetKey(string dep,int year,string cat)=>$"budget:{dep}:{year}:{cat}";
    public static ProposalView View(BudgetProposal p)=>new(p.Id,p.DepartmentId,p.Year,p.Category,p.Title,p.Purpose,p.OwnerId,p.Status,p.Amount,JsonSerializer.Deserialize<ProposalLine[]>(p.LinesJson)!,p.ReviewNote,p.ReviewerId,Convert.ToBase64String(p.RowVersion));
    public static PaymentView View(PaymentRequest p)=>new(p.Id,p.DepartmentId,p.Year,p.Amount,p.Content,p.MaintenanceId,p.Status,Convert.ToBase64String(p.RowVersion));
    public static FinanceEntryView View(FinanceEntry e)=>new(e.Id,e.DepartmentId,e.Year,e.Category,e.Kind,e.Amount,e.Voucher,e.Date,e.Content,e.PaymentRequestId,e.FundingProposalId);
    public async Task<ProposalView[]> ProposalViews(BudgetProposal[] rows,CancellationToken ct)
    {
        var ids=rows.Select(p=>p.Id).ToArray();
        var receipts=await db.FinanceEntries.AsNoTracking().Where(e=>ids.Contains(e.FundingProposalId!)&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).ToDictionaryAsync(e=>e.FundingProposalId!,e=>e.Id,ct);
        var events=await db.AuditEvents.AsNoTracking().Where(x=>ids.Contains(x.TargetId)&&(x.Action=="proposal.appraise"||x.Action=="proposal.save")).OrderByDescending(x=>x.Id).ToArrayAsync(ct);
        return rows.Select(p=>{var a=events.FirstOrDefault(x=>x.TargetId==p.Id);if(a?.Action!="proposal.appraise")a=null;var note=a==null?null:JsonSerializer.Deserialize<ProposalCommand>(a.DetailJson)?.Note;return View(p) with{FundingReceiptId=receipts.GetValueOrDefault(p.Id),AppraisalNote=note,AppraiserId=a?.ActorId,AppraisedAt=a?.At};}).ToArray();
    }
    public Task<FinanceEntryView> ReceiveFunding(string actor,string id,string key,ReceiveProposalFunding input,CancellationToken ct)=>commands.Run(actor,key,"proposal.receive:"+id,input,
        async()=>{await GetProposal(actor,id,"transactions.write",ct);},async()=>{
            await TransactionLocks.Acquire(db,"proposal:"+id,ct);
            var p=await db.BudgetProposals.SingleAsync(x=>x.Id==id,ct);
            await Demand(actor,"transactions.write",p.DepartmentId,ct);AdminService.Version(p.RowVersion,input.Version);
            if(p.Status!="approved")throw AppException.Invalid("Chỉ nhận kinh phí của dự toán đã được duyệt.");
            if(input.Date.Year!=p.Year)throw AppException.Invalid("Ngày nhận tiền phải thuộc năm tài chính của dự toán.");
            await FinanceControlService.Open(db,p.DepartmentId,p.Year,ct);
            await TransactionLocks.Acquire(db,"finance-vouchers",ct);
            if(await db.FinanceEntries.AnyAsync(e=>e.FundingProposalId==id&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id),ct))
                throw new AppException(409,"FUNDING_RECEIVED","Dự toán đã có phiếu thu nhận kinh phí.");
            var voucher=AdminService.Required(input.Voucher,100);
            if(await db.FinanceEntries.AnyAsync(e=>e.Year==p.Year&&e.Voucher==voucher,ct))throw new AppException(409,"VOUCHER_EXISTS","Số chứng từ đã được dùng trong năm tài chính.");
            var content="Nhận kinh phí dự toán: "+p.Title;
            var entry=new FinanceEntry{DepartmentId=p.DepartmentId,Year=p.Year,Category=p.Category,Kind="receipt",Amount=p.Amount,Voucher=voucher,Date=input.Date,Content=content[..Math.Min(2000,content.Length)],ActorId=actor,FundingProposalId=p.Id};
            db.FinanceEntries.Add(entry);admin.Audit(actor,"proposal.receive",p.Id,new{entry.Id,entry.Amount,input.Voucher,input.Date});
            await db.SaveChangesAsync(ct);return View(entry);
        },ct);
    public async Task<BudgetProposal> GetProposal(string actor,string id,string permission,CancellationToken ct)
    {
        var p=await db.BudgetProposals.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await Demand(actor,permission,p.DepartmentId,ct);return p;
    }
    public Task<ProposalView> Save(string actor,string? id,string key,SaveProposal input,CancellationToken ct)=>commands.Run(actor,key,"proposal.save:"+(id??"new"),input,
        async()=>{await Demand(actor,"budget.write",input.DepartmentId,ct);if(id!=null)await GetProposal(actor,id,"budget.write",ct);},async()=>{
            Year(input.Year);Category(input.Category);var department=await Department(input.DepartmentId,ct);
            if(input.Lines==null||input.Lines.Length is <1 or >100||input.Lines.Any(l=>l==null))throw AppException.Invalid("Dự toán cần từ 1 đến 100 khoản chi tiết hợp lệ.");
            var lines=input.Lines.Select(l=>new ProposalLine(AdminService.Required(l.Name,150),l.Quantity,l.Price)).ToArray();
            foreach(var l in lines){if(l.Quantity is <1 or >1000000)throw AppException.Invalid("Số lượng không hợp lệ.");Money(l.Price);}
            var amount=lines.Sum(l=>l.Quantity*l.Price);Money(amount);
            BudgetProposal p;
            if(id==null){p=new(){OwnerId=actor};db.BudgetProposals.Add(p);}
            else{
                await TransactionLocks.Acquire(db,"proposal:"+id,ct);p=await db.BudgetProposals.SingleAsync(x=>x.Id==id,ct);
                await Demand(actor,"budget.write",p.DepartmentId,ct);AdminService.Version(p.RowVersion,input.Version??"");
                if(p.Status is not("draft" or "returned"))throw AppException.Invalid("Chỉ sửa dự toán nháp hoặc trả bổ sung.");
            }
            var periods=new[]{(Department:department,Year:input.Year),(Department:id==null?department:p.DepartmentId,Year:id==null?input.Year:p.Year)};
            foreach(var period in periods.Distinct().OrderBy(x=>FinanceControlService.PeriodKey(x.Department,x.Year),StringComparer.Ordinal))
                await FinanceControlService.Open(db,period.Department,period.Year,ct);
            p.DepartmentId=department;p.Year=input.Year;p.Category=input.Category;p.Title=AdminService.Required(input.Title,150);
            p.Purpose=AdminService.OptionalNote(input.Purpose);p.LinesJson=JsonSerializer.Serialize(lines);p.Amount=amount;p.Status="draft";
            p.ReviewNote="";p.ReviewerId=null;admin.Audit(actor,"proposal.save",p.Id,input);await db.SaveChangesAsync(ct);return View(p);
        },ct);
    public Task<ProposalView> Action(string actor,string id,string operation,string key,ProposalCommand input,CancellationToken ct)
    {
        var permission=operation=="submit"?"budget.write":operation is "appraise" or "return-review"?"budget.appraise":"budget.approve";
        return commands.Run(actor,key,"proposal."+operation+":"+id,input,async()=>{await GetProposal(actor,id,permission,ct);},async()=>{
            await TransactionLocks.Acquire(db,"proposal:"+id,ct);var p=await db.BudgetProposals.SingleAsync(x=>x.Id==id,ct);
            await Demand(actor,permission,p.DepartmentId,ct);AdminService.Version(p.RowVersion,input.Version);
            await FinanceControlService.Open(db,p.DepartmentId,p.Year,ct);
            if(operation=="submit"){
                if(p.Status!="draft")throw AppException.Invalid("Chỉ gửi dự toán nháp.");p.Status="reviewing";
            }else if(operation is "appraise" or "return-review"){
                if(p.Status is not("reviewing" or "pending"))throw AppException.Invalid("Dự toán không còn chờ thẩm định.");
                p.ReviewNote=AdminService.Required(input.Note??"",2000);p.ReviewerId=actor;
                p.Status=operation=="appraise"?"pending":"returned";
            }else{
                if(operation is not("approve" or "reject" or "return")||p.Status!="pending")throw AppException.Invalid("Dự toán không còn chờ duyệt.");
                var last=await db.AuditEvents.AsNoTracking().Where(x=>x.TargetId==id&&x.Action.StartsWith("proposal.")).OrderByDescending(x=>x.Id).Select(x=>x.Action).FirstOrDefaultAsync(ct);
                if(last!="proposal.appraise")throw AppException.Invalid("Dự toán cần được KHTC thẩm định và trình trước khi quyết định.");
                p.ReviewNote=AdminService.Required(input.Note??"",2000);p.ReviewerId=actor;p.Status=operation=="approve"?"approved":operation=="return"?"returned":"rejected";
                if(operation=="approve"){
                    await TransactionLocks.Acquire(db,BudgetKey(p.DepartmentId,p.Year,p.Category),ct);
                    db.BudgetAllocations.Add(new(){ProposalId=p.Id,Amount=p.Amount});
                }
            }
            admin.Audit(actor,"proposal."+operation,id,input);await db.SaveChangesAsync(ct);return View(p);
        },ct);
    }
    public async Task<BudgetBalance> Balance(string actor,string department,int year,string category,CancellationToken ct)
    {
        await Demand(actor,"budget.read",department,ct);Year(year);Category(category);
        department=await Department(department,ct,false);
        await using var tx=await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db,BudgetKey(department,year,category),ct);
        var result=await Calculate(department,year,category,ct);await tx.CommitAsync(ct);return result;
    }
    public async Task<BudgetBalance> Calculate(string dep,int year,string cat,CancellationToken ct)
    {
        var allocated=await (from a in db.BudgetAllocations
            join p in db.BudgetProposals on a.ProposalId equals p.Id
            where p.DepartmentId==dep&&p.Year==year&&p.Category==cat
            select (decimal?)a.Amount).SumAsync(ct)??0;
        var spent=await db.FinanceEntries.Where(x=>x.DepartmentId==dep&&x.Year==year&&x.Category==cat&&x.Kind=="expense"&&!db.FinanceReversals.Any(r=>r.EntryId==x.Id)).SumAsync(x=>(decimal?)x.Amount,ct)??0;
        allocated+=await db.BudgetAdjustments.Where(x=>x.DepartmentId==dep&&x.Year==year&&x.Category==cat&&x.Status=="approved").SumAsync(x=>(decimal?)x.Amount,ct)??0;
        return new(dep,year,cat,allocated,spent,allocated-spent);
    }
    public async Task<FinanceSummary> Summary(string actor,int year,string? department,string? category,CancellationToken ct)
    {
        Year(year);if(!string.IsNullOrEmpty(category))Category(category);
        var profile=await access.GetProfile(actor,ct);
        if(!AccessRules.HasPermission(profile,"budget.read"))throw AppException.Forbidden();
        if(!string.IsNullOrEmpty(department))await Demand(actor,"budget.read",department,ct);
        // A single SQL statement keeps allocations, adjustments and effective entries at the same read point.
        var allocations=from p in AssetOperationsService.Scoped(db.BudgetProposals.AsNoTracking(),profile,"budget.read")
            where p.Year==year&&(department==null||department==""||p.DepartmentId==department)&&(category==null||category==""||p.Category==category)
            join a in db.BudgetAllocations on p.Id equals a.ProposalId
            select new{p.DepartmentId,p.Category,Allocated=a.Amount,Spent=0m,Received=0m};
        var adjustments=AssetOperationsService.Scoped(db.BudgetAdjustments.AsNoTracking(),profile,"budget.read")
            .Where(x=>x.Year==year&&x.Status=="approved"&&(department==null||department==""||x.DepartmentId==department)&&(category==null||category==""||x.Category==category))
            .Select(x=>new{x.DepartmentId,x.Category,Allocated=x.Amount,Spent=0m,Received=0m});
        var entries=AssetOperationsService.Scoped(db.FinanceEntries.AsNoTracking(),profile,"budget.read")
            .Where(x=>x.Year==year&&!db.FinanceReversals.Any(r=>r.EntryId==x.Id)&&(department==null||department==""||x.DepartmentId==department)&&(category==null||category==""||x.Category==category))
            .Select(x=>new{x.DepartmentId,x.Category,Allocated=0m,Spent=x.Kind=="expense"?x.Amount:0m,Received=x.Kind=="receipt"?x.Amount:0m});
        var rows=await allocations.Concat(adjustments).Concat(entries).GroupBy(x=>new{x.DepartmentId,x.Category})
            .Select(g=>new{g.Key.DepartmentId,g.Key.Category,Allocated=g.Sum(x=>x.Allocated),Spent=g.Sum(x=>x.Spent),Received=g.Sum(x=>x.Received)})
            .OrderBy(x=>x.DepartmentId).ThenBy(x=>x.Category).ToArrayAsync(ct);
        return new(rows.Select(x=>new BudgetBalance(x.DepartmentId,year,x.Category,x.Allocated,x.Spent,x.Allocated-x.Spent)).ToArray(),rows.Sum(x=>x.Received));
    }
    public Task<PaymentView> CreatePayment(string actor,string key,CreatePaymentRequest input,CancellationToken ct)=>commands.Run(actor,key,"payment.create",input,
        ()=>Demand(actor,"transactions.write",input.DepartmentId,ct),async()=>{
            Year(input.Year);Money(input.Amount);var department=await Department(input.DepartmentId,ct);
            await FinanceControlService.Open(db,department,input.Year,ct);
            var p=new PaymentRequest{DepartmentId=department,Year=input.Year,Amount=input.Amount,Content=AdminService.Required(input.Content,2000)};
            db.PaymentRequests.Add(p);admin.Audit(actor,"payment.create",p.Id,input);await db.SaveChangesAsync(ct);return View(p);
        },ct);
    public Task<PaymentView> ImportMaintenance(string actor,string id,string key,ImportMaintenanceCost input,CancellationToken ct)=>commands.Run(actor,key,"payment.import:"+id,input,
        async()=>{
            var m=await db.MaintenanceOrders.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
            await Demand(actor,"transactions.write",m.DepartmentId,ct);
        },async()=>{
            Year(input.Year);var snapshot=await db.MaintenanceOrders.AsNoTracking().SingleAsync(x=>x.Id==id,ct);
            await TransactionLocks.Acquire(db,"asset:"+snapshot.AssetGroupId,ct);
            var m=await db.MaintenanceOrders.AsNoTracking().SingleAsync(x=>x.Id==id,ct);
            await Demand(actor,"transactions.write",m.DepartmentId,ct);
            if(m.Status!="completed"||m.Cost<=0)throw AppException.Invalid("Bảo trì chưa hoàn tất hoặc không phát sinh chi phí.");
            var existing=await db.PaymentRequests.SingleOrDefaultAsync(x=>x.MaintenanceId==id,ct);
            if(existing!=null){if(existing.Year!=input.Year)throw AppException.Invalid("Chi phí đã có yêu cầu thuộc năm khác.");return View(existing) with{Paid=await Paid(existing.Id,ct)};}
            await FinanceControlService.Open(db,await Department(m.DepartmentId,ct,false),input.Year,ct);
            var content="Chi phí bảo trì: "+m.Content;
            var p=new PaymentRequest{DepartmentId=await Department(m.DepartmentId,ct,false),Year=input.Year,Amount=m.Cost,Content=content[..Math.Min(2000,content.Length)],MaintenanceId=m.Id};
            db.PaymentRequests.Add(p);admin.Audit(actor,"payment.import",p.Id,new{id,input.Year,m.Cost});await db.SaveChangesAsync(ct);return View(p);
        },ct);
    public Task<FinanceEntryView> Record(string actor,string kind,string key,RecordFinanceEntry input,CancellationToken ct)=>commands.Run(actor,key,"finance."+kind,input,
        ()=>Demand(actor,"transactions.write",input.DepartmentId,ct),async()=>{
            if(kind is not("receipt" or "expense"))throw AppException.Invalid("Loại giao dịch không hợp lệ.");
            Year(input.Year);Category(input.Category);Money(input.Amount);var department=await Department(input.DepartmentId,ct);
            if(input.Date.Year!=input.Year)throw AppException.Invalid("Ngày chứng từ phải thuộc năm tài chính.");
            var voucher=AdminService.Required(input.Voucher,100);
            PaymentRequest? payment=null;
            // Lock order: source request, budget, voucher. All writers use this order.
            if(input.PaymentRequestId!=null){
                if(kind!="expense")throw AppException.Invalid("Chỉ khoản chi được thanh toán yêu cầu.");
                await TransactionLocks.Acquire(db,"payment:"+input.PaymentRequestId,ct);
                payment=await db.PaymentRequests.SingleOrDefaultAsync(x=>x.Id==input.PaymentRequestId,ct)??throw AppException.Missing();
                await Demand(actor,"transactions.write",payment.DepartmentId,ct);AdminService.Version(payment.RowVersion,input.Version??"");
                if(payment.Status is not("pending" or "part_paid")||await Department(payment.DepartmentId,ct,false)!=department||payment.Year!=input.Year||input.Amount>payment.Amount-await Paid(payment.Id,ct))throw AppException.Invalid("Yêu cầu đã xử lý hoặc số tiền, đơn vị, năm không khớp.");
            }
            await FinanceControlService.Open(db,department,input.Year,ct);
            if(kind=="expense"){
                await TransactionLocks.Acquire(db,BudgetKey(department,input.Year,input.Category),ct);
                var balance=await Calculate(department,input.Year,input.Category,ct);
                if(balance.Available<input.Amount)throw new AppException(409,"BUDGET_EXCEEDED","Vượt ngân sách khả dụng; yêu cầu chờ chưa được tính là khoản chi.");
            }
            // SQL collation may be accent-insensitive; serialize the short voucher registry globally.
            await TransactionLocks.Acquire(db,"finance-vouchers",ct);
            if(await db.FinanceEntries.AnyAsync(x=>x.Year==input.Year&&x.Voucher==voucher,ct))throw new AppException(409,"VOUCHER_EXISTS","Số chứng từ đã được dùng trong năm tài chính.");
            var e=new FinanceEntry{DepartmentId=department,Year=input.Year,Category=input.Category,Kind=kind,Amount=input.Amount,Voucher=voucher,Date=input.Date,Content=AdminService.Required(input.Content,2000),ActorId=actor,PaymentRequestId=payment?.Id};
            db.FinanceEntries.Add(e);if(payment!=null)payment.Status=await Paid(payment.Id,ct)+input.Amount==payment.Amount?"paid":"part_paid";
            admin.Audit(actor,"finance."+kind,e.Id,new{input,e.PaymentRequestId});await db.SaveChangesAsync(ct);return View(e);
        },ct);
    public async Task<decimal> Paid(string paymentId,CancellationToken ct)=>await db.FinanceEntries.Where(e=>e.PaymentRequestId==paymentId&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).SumAsync(e=>(decimal?)e.Amount,ct)??0;
    public async Task<PaymentView[]> PaymentViews(PaymentRequest[] rows,CancellationToken ct)
    {
        var ids=rows.Select(x=>x.Id).ToArray();
        var paid=await db.FinanceEntries.AsNoTracking().Where(e=>ids.Contains(e.PaymentRequestId!)&&!db.FinanceReversals.Any(r=>r.EntryId==e.Id)).GroupBy(e=>e.PaymentRequestId).Select(g=>new{Id=g.Key,Amount=g.Sum(e=>e.Amount)}).ToDictionaryAsync(x=>x.Id!,x=>x.Amount,ct);
        return rows.Select(p=>View(p) with{Paid=paid.GetValueOrDefault(p.Id)}).ToArray();
    }
    // Called within the maintenance completion transaction. Never records an expense by itself.
    public static async Task QueueMaintenance(HpuniDbContext db,MaintenanceOrder m,CancellationToken ct)
    {
        if(m.Cost<=0)return;
        var content="Chi phí bảo trì: "+m.Content;
        var department=await db.Departments.Where(d=>d.Id==m.DepartmentId).Select(d=>d.Id).SingleAsync(ct);
        db.PaymentRequests.Add(new(){DepartmentId=department,Year=DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).Year,
            Amount=m.Cost,Content=content[..Math.Min(2000,content.Length)],MaintenanceId=m.Id});
    }
}
