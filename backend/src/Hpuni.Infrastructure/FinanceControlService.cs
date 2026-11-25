using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class FinanceControlService(HpuniDbContext db, FinanceService finance, AdminService admin, IdempotentCommands commands)
{
    public static string PeriodKey(string department,int year)=>$"fiscal:{department}:{year}";
    public static async Task Open(HpuniDbContext db,string department,int year,CancellationToken ct)
    {
        await TransactionLocks.Acquire(db,PeriodKey(department,year),ct);
        if(await db.FiscalPeriods.AnyAsync(x=>x.DepartmentId==department&&x.Year==year&&x.Closed,ct))
            throw new AppException(409,"PERIOD_CLOSED","Năm tài chính của đơn vị đã khóa sổ.");
    }
    static void Year(int year){if(year is <2000 or >2100)throw AppException.Invalid("Năm tài chính không hợp lệ.");}
    async Task<string> Department(string department,CancellationToken ct)=>await db.Departments.Where(x=>x.Id==department).Select(x=>x.Id).SingleOrDefaultAsync(ct)??throw AppException.Invalid("Đơn vị không tồn tại.");
    public static BudgetAdjustmentView View(BudgetAdjustment a)=>new(a.Id,a.DepartmentId,a.Year,a.Category,a.Amount,a.Reason,a.Status,a.ActorId,a.ReviewerId,a.ReviewNote,Convert.ToBase64String(a.RowVersion));
    public static FiscalPeriodView View(FiscalPeriod p)=>new(p.DepartmentId,p.Year,p.Closed,p.Reason,p.ChangedAt,Convert.ToBase64String(p.RowVersion));
    public async Task<FiscalPeriodView> Period(string actor,string department,int year,CancellationToken ct)
    {
        Year(year);await finance.Demand(actor,"budget.read",department,ct);department=await Department(department,ct);
        var p=await db.FiscalPeriods.AsNoTracking().SingleOrDefaultAsync(x=>x.DepartmentId==department&&x.Year==year,ct);
        return p==null?new(department,year,false,"",null,null):View(p);
    }
    public Task<FiscalPeriodView> ChangePeriod(string actor,string key,ChangeFiscalPeriod input,CancellationToken ct)=>commands.Run(actor,key,"fiscal.change",input,
        ()=>finance.Demand(actor,"transactions.write",input.DepartmentId,ct),async()=>{
            Year(input.Year);var dep=await Department(input.DepartmentId,ct);var reason=AdminService.Required(input.Reason,2000);
            await TransactionLocks.Acquire(db,PeriodKey(dep,input.Year),ct);
            var p=await db.FiscalPeriods.SingleOrDefaultAsync(x=>x.DepartmentId==dep&&x.Year==input.Year,ct);
            if(p==null){if(!string.IsNullOrEmpty(input.Version))throw new AppException(409,"STALE_VERSION","Trạng thái khóa sổ đã thay đổi.");p=new(){DepartmentId=dep,Year=input.Year};db.FiscalPeriods.Add(p);}
            else AdminService.Version(p.RowVersion,input.Version??"");
            if(p.Closed==input.Closed)throw AppException.Invalid("Năm tài chính đã ở trạng thái yêu cầu.");
            p.Closed=input.Closed;p.ActorId=actor;p.Reason=reason;p.ChangedAt=DateTimeOffset.UtcNow;
            admin.Audit(actor,input.Closed?"fiscal.close":"fiscal.reopen",dep+":"+input.Year,input);
            await db.SaveChangesAsync(ct);return View(p);
        },ct);
    public Task<BudgetAdjustmentView> CreateAdjustment(string actor,string key,CreateBudgetAdjustment input,CancellationToken ct)=>commands.Run(actor,key,"budget.adjustment.create",input,
        ()=>finance.Demand(actor,"budget.write",input.DepartmentId,ct),async()=>{
            Year(input.Year);if(!FinanceService.Categories.Contains(input.Category)||input.Amount==0||decimal.Truncate(input.Amount)!=input.Amount||(input.Amount < -1000000000000m || input.Amount > 1000000000000m))throw AppException.Invalid("Khoản mục hoặc số tiền điều chỉnh không hợp lệ.");
            var dep=await Department(input.DepartmentId,ct);await Open(db,dep,input.Year,ct);
            var a=new BudgetAdjustment{DepartmentId=dep,Year=input.Year,Category=input.Category,Amount=input.Amount,Reason=AdminService.Required(input.Reason,2000),ActorId=actor};
            db.BudgetAdjustments.Add(a);admin.Audit(actor,"budget.adjustment.create",a.Id,input);await db.SaveChangesAsync(ct);return View(a);
        },ct);
    public Task<BudgetAdjustmentView> ReviewAdjustment(string actor,string id,string key,ReviewBudgetAdjustment input,CancellationToken ct)=>commands.Run(actor,key,"budget.adjustment.review:"+id,input,
        async()=>{var a=await db.BudgetAdjustments.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();await finance.Demand(actor,"budget.approve",a.DepartmentId,ct);},async()=>{
            await TransactionLocks.Acquire(db,"adjustment:"+id,ct);var a=await db.BudgetAdjustments.SingleAsync(x=>x.Id==id,ct);
            await finance.Demand(actor,"budget.approve",a.DepartmentId,ct);AdminService.Version(a.RowVersion,input.Version);
            if(a.Status!="pending")throw AppException.Invalid("Điều chỉnh không còn chờ duyệt.");
            if(a.ActorId==actor)throw AppException.Invalid("Người lập không được tự duyệt điều chỉnh.");
            await Open(db,a.DepartmentId,a.Year,ct);
            await TransactionLocks.Acquire(db,$"budget:{a.DepartmentId}:{a.Year}:{a.Category}",ct);
            if(input.Approve){var balance=await finance.Calculate(a.DepartmentId,a.Year,a.Category,ct);if(balance.Available+a.Amount<0)throw new AppException(409,"BUDGET_EXCEEDED","Không được giảm ngân sách xuống dưới số đã chi.");}
            a.Status=input.Approve?"approved":"rejected";a.ReviewerId=actor;a.ReviewNote=AdminService.Required(input.Note,2000);
            admin.Audit(actor,"budget.adjustment.review",id,input);await db.SaveChangesAsync(ct);return View(a);
        },ct);
    public Task<FinanceReversalView> Reverse(string actor,string id,string key,ReverseFinanceEntry input,CancellationToken ct)=>commands.Run(actor,key,"finance.reverse:"+id,input,
        async()=>{var e=await db.FinanceEntries.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();await finance.Demand(actor,"transactions.write",e.DepartmentId,ct);},async()=>{
            var e=await db.FinanceEntries.AsNoTracking().SingleAsync(x=>x.Id==id,ct);
            // Same lock order as payment recording: source, year, budget, voucher registry.
            if(e.PaymentRequestId!=null)await TransactionLocks.Acquire(db,"payment:"+e.PaymentRequestId,ct);
            if(e.FundingProposalId!=null)await TransactionLocks.Acquire(db,"proposal:"+e.FundingProposalId,ct);
            await Open(db,e.DepartmentId,e.Year,ct);
            await TransactionLocks.Acquire(db,$"budget:{e.DepartmentId}:{e.Year}:{e.Category}",ct);
            await TransactionLocks.Acquire(db,"finance-vouchers",ct);
            if(await db.FinanceReversals.AnyAsync(x=>x.EntryId==id,ct))throw new AppException(409,"ALREADY_REVERSED","Chứng từ đã được đảo.");
            var r=new FinanceReversal{EntryId=id,Reason=AdminService.Required(input.Reason,2000),ActorId=actor};db.FinanceReversals.Add(r);
            if(e.PaymentRequestId!=null){
                var p=await db.PaymentRequests.SingleAsync(x=>x.Id==e.PaymentRequestId,ct);
                var paid=await finance.Paid(p.Id,ct)-e.Amount;p.Status=paid==0?"pending":paid<p.Amount?"part_paid":"paid";
            }
            admin.Audit(actor,"finance.reverse",id,new{r.Id,input.Reason});await db.SaveChangesAsync(ct);
            return new FinanceReversalView(r.Id,r.EntryId,r.Reason,r.ActorId,r.CreatedAt);
        },ct);
}
