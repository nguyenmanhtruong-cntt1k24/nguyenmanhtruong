using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class FinanceControlsTests(FinanceFactory f):IClassFixture<FinanceFactory>
{
    static Task<HttpResponseMessage> Post(HttpClient c,string path,object data,string? key=null){var r=new HttpRequestMessage(HttpMethod.Post,"/api/v1/finance/"+path){Content=JsonContent.Create(data)};r.Headers.Add("Idempotency-Key",key??Guid.NewGuid().ToString());return c.SendAsync(r);}
    static async Task<T> Ok<T>(Task<HttpResponseMessage> task){var r=await task;Assert.True(r.IsSuccessStatusCode,await r.Content.ReadAsStringAsync());return (await r.Content.ReadFromJsonAsync<T>())!;}
    async Task<HttpClient> User(string role,string scope="all"){
        var root=await f.Admin();var u=await SqlFactory.Create(root,role);
        Assert.Equal(HttpStatusCode.OK,(await root.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope(scope,[],[],u.Version))).StatusCode);
        return await f.Login(u.UserName);
    }
    async Task Fund(HttpClient a,HttpClient e,int year,decimal amount){
        var p=await Ok<ProposalView>(Post(a,"proposals",new SaveProposal("it",year,"general","Ngân sách",null,[new("Khoản",1,amount)])));
        p=await Ok<ProposalView>(Post(a,$"proposals/{p.Id}/submit",new ProposalCommand(p.Version)));
        await Ok<ProposalView>(Post(e,$"proposals/{p.Id}/approve",new ProposalCommand(p.Version,"Duyệt")));
    }
    static RecordFinanceEntry Entry(int year,decimal amount)=>new("it",year,"general",amount,Guid.NewGuid().ToString(),new(year,1,1),"Kiểm thử");
    [Fact]
    public async Task GuidCaseAliasesUseTheSameTransactionLock()
    {
        await f.Admin();await using var first=f.Services.CreateAsyncScope();await using var second=f.Services.CreateAsyncScope();
        var a=first.ServiceProvider.GetRequiredService<HpuniDbContext>();var b=second.ServiceProvider.GetRequiredService<HpuniDbContext>();
        await using var tx=await a.Database.BeginTransactionAsync();await using var other=await b.Database.BeginTransactionAsync();
        const string id="aabbccdd-1234-4567-8901-aabbccddeeff";
        await TransactionLocks.Acquire(a,"asset:"+id);var waiting=TransactionLocks.Acquire(b,"asset:"+id.ToUpperInvariant());
        await Task.Delay(150);Assert.False(waiting.IsCompleted);await tx.CommitAsync();await waiting;await other.CommitAsync();
    }
    [Fact]
    public async Task PartialPaymentsAreSerializedAndReversalRestoresOutstandingWithoutChangingOriginal(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2041;await Fund(a,e,year,100);
        var p=await Ok<PaymentView>(Post(a,"payments",new CreatePaymentRequest("it",year,100,"Trả từng phần")));
        var first=await Ok<FinanceEntryView>(Post(a,"entries/expense",Entry(year,40) with{PaymentRequestId=p.Id,Version=p.Version}));
        var list=await a.GetFromJsonAsync<JsonElement>("/api/v1/finance/payments?pageSize=100");
        p=list.GetProperty("items").EnumerateArray().Single(x=>x.GetProperty("id").GetString()==p.Id).Deserialize<PaymentView>(new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.Equal("part_paid",p.Status);Assert.Equal(40,p.Paid);Assert.Equal(60,p.Remaining);
        var input=Entry(year,60) with{PaymentRequestId=p.Id,Version=p.Version};
        var responses=await Task.WhenAll(Post(a,"entries/expense",input),Post(a,"entries/expense",input with{Voucher=Guid.NewGuid().ToString()}));
        Assert.Single(responses,r=>r.StatusCode==HttpStatusCode.OK);Assert.Single(responses,r=>r.StatusCode==HttpStatusCode.Conflict);
        var key=Guid.NewGuid().ToString();var reason=new ReverseFinanceEntry("Sai chứng từ, ghi lại sau");
        var reversal=await Ok<FinanceReversalView>(Post(e,$"entries/{first.Id}/reverse",reason,key));
        Assert.Equal(reversal.Id,(await Ok<FinanceReversalView>(Post(e,$"entries/{first.Id}/reverse",reason,key))).Id);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(e,$"entries/{first.Id}/reverse",reason)).StatusCode);
        var b=await a.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}");Assert.Equal(40,b!.Available);
        await using var scope=f.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(40,(await db.FinanceEntries.SingleAsync(x=>x.Id==first.Id)).Amount);Assert.Equal("part_paid",(await db.PaymentRequests.SingleAsync(x=>x.Id==p.Id)).Status);
        var report=await a.GetFromJsonAsync<FinanceReport>($"/api/v1/reports/finance?year={year}");Assert.Equal(60,Assert.Single(report!.Rows).Spent);Assert.Equal(40,Assert.Single(report.PendingCosts).Pending);
    }
    [Fact]
    public async Task ClosingBlocksWritersAndReversalReopenRequiresFreshVersion(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2042;await Fund(a,e,year,100);
        var entry=await Ok<FinanceEntryView>(Post(a,"entries/expense",Entry(year,20)));
        var closed=await Ok<FiscalPeriodView>(Post(e,"period",new ChangeFiscalPeriod("IT",year,true,"Chốt năm")));
        Assert.True(closed.Closed);
        Assert.Equal(HttpStatusCode.Forbidden,(await Post(a,"period",new ChangeFiscalPeriod("it",year,false,"Mở lại",closed.Version))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"entries/receipt",Entry(year,1))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(e,$"entries/{entry.Id}/reverse",new ReverseFinanceEntry("Sửa"))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"adjustments",new CreateBudgetAdjustment("it",year,"general",10,"Thêm"))).StatusCode);
        var opened=await Ok<FiscalPeriodView>(Post(e,"period",new ChangeFiscalPeriod("it",year,false,"Kiểm tra lại",closed.Version)));
        Assert.Equal(HttpStatusCode.Conflict,(await Post(e,"period",new ChangeFiscalPeriod("it",year,true,"Chốt lại",closed.Version))).StatusCode);
        Assert.False(opened.Closed);await Ok<FinanceEntryView>(Post(a,"entries/receipt",Entry(year,1)));
    }
    [Fact]
    public async Task AdjustmentsNeedSeparateReviewerAndCannotReduceBelowSpent(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2043;await Fund(a,e,year,100);
        await Ok<FinanceEntryView>(Post(a,"entries/expense",Entry(year,70)));
        var adj=await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("IT",year,"general",-31,"Giảm")));
        Assert.Equal(HttpStatusCode.Conflict,(await Post(e,$"adjustments/{adj.Id}/review",new ReviewBudgetAdjustment(adj.Version,true,"Duyệt"))).StatusCode);
        await Ok<BudgetAdjustmentView>(Post(e,$"adjustments/{adj.Id}/review",new ReviewBudgetAdjustment(adj.Version,false,"Vượt dư")));
        adj=await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("it",year,"general",-30,"Giảm đúng dư")));
        var key=Guid.NewGuid().ToString();var cmd=new ReviewBudgetAdjustment(adj.Version,true,"Duyệt giảm");
        await Ok<BudgetAdjustmentView>(Post(e,$"adjustments/{adj.Id}/review",cmd,key));await Ok<BudgetAdjustmentView>(Post(e,$"adjustments/{adj.Id}/review",cmd,key));
        var b=await a.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}");Assert.Equal(70,b!.Allocated);Assert.Equal(0,b.Available);
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var role=await db.Roles.SingleAsync(r=>r.Id=="executive");
            db.RolePermissions.Add(new(){RoleId=role.Id,Permission="budget.write"});await db.SaveChangesAsync();
        }
        var own=await Ok<BudgetAdjustmentView>(Post(e,"adjustments",new CreateBudgetAdjustment("it",year,"general",10,"Tự lập")));
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(e,$"adjustments/{own.Id}/review",new ReviewBudgetAdjustment(own.Version,true,"Tự duyệt"))).StatusCode);
    }
    [Fact]
    public async Task SummaryIncludesAdjustmentOnlyBudgetAndListsPageAfterFiltering()
    {
        using var a=await User("accounting");using var e=await User("executive");const int year=2051;
        var adj=await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("it",year,"equipment",150,"Khoản mới chưa có dự toán")));
        await Ok<BudgetAdjustmentView>(Post(e,$"adjustments/{adj.Id}/review",new ReviewBudgetAdjustment(adj.Version,true,"Duyệt cấp bổ sung")));
        var summary=await a.GetFromJsonAsync<FinanceSummary>($"/api/v1/finance/summary?year={year}&departmentId=it&category=equipment");
        Assert.Equal(150,Assert.Single(summary!.Balances).Available);Assert.Equal(0,summary.Received);
        var expense=await Ok<FinanceEntryView>(Post(a,"entries/expense",Entry(year,40) with{Category="equipment"}));
        await Ok<FinanceEntryView>(Post(a,"entries/receipt",Entry(year,25) with{Category="equipment"}));
        summary=await a.GetFromJsonAsync<FinanceSummary>($"/api/v1/finance/summary?year={year}");
        Assert.Equal(110,Assert.Single(summary!.Balances).Available);Assert.Equal(25,summary.Received);
        await Ok<FinanceReversalView>(Post(e,$"entries/{expense.Id}/reverse",new ReverseFinanceEntry("Đảo kiểm thử tổng hợp")));
        summary=await a.GetFromJsonAsync<FinanceSummary>($"/api/v1/finance/summary?year={year}");
        Assert.Equal(150,Assert.Single(summary!.Balances).Available);
        for(var i=0;i<3;i++)await Ok<PaymentView>(Post(a,"payments",new CreatePaymentRequest("it",year,10,"Phân trang "+i)));
        var page=await a.GetFromJsonAsync<JsonElement>($"/api/v1/finance/payments?year={year}&departmentId=it&page=2&pageSize=2");
        Assert.Equal(3,page.GetProperty("total").GetInt32());Assert.Single(page.GetProperty("items").EnumerateArray());
        var empty=await a.GetFromJsonAsync<JsonElement>($"/api/v1/finance/entries?year={year}&category=teaching");
        Assert.Equal(0,empty.GetProperty("total").GetInt32());
        using var scoped=await User("accounting","unit");
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync($"/api/v1/finance/summary?year={year}&departmentId=economics")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync($"/api/v1/finance/payments?year={year}&departmentId=economics")).StatusCode);
    }
    [Fact]
    public async Task AdjustmentFiltersRunBeforePagingAndSinglePaymentKeepsScope()
    {
        using var a=await User("accounting");const int year=2054;
        for(var i=0;i<3;i++)await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("it",year,"equipment",10,"Phân trang điều chỉnh "+i)));
        await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("it",year,"maintenance",10,"Khoản khác")));
        await Ok<BudgetAdjustmentView>(Post(a,"adjustments",new CreateBudgetAdjustment("it",year+1,"equipment",10,"Năm khác")));
        var page=await a.GetFromJsonAsync<JsonElement>($"/api/v1/finance/adjustments?year={year}&departmentId=it&category=equipment&page=2&pageSize=2");
        Assert.Equal(3,page.GetProperty("total").GetInt32());
        var item=Assert.Single(page.GetProperty("items").EnumerateArray());
        Assert.Equal(year,item.GetProperty("year").GetInt32());Assert.Equal("equipment",item.GetProperty("category").GetString());
        Assert.Equal(HttpStatusCode.BadRequest,(await a.GetAsync("/api/v1/finance/adjustments?year=1999")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await a.GetAsync("/api/v1/finance/adjustments?category=unknown")).StatusCode);
        var payment=await Ok<PaymentView>(Post(a,"payments",new CreatePaymentRequest("it",year,10,"Đọc một hồ sơ")));
        var read=await a.GetFromJsonAsync<PaymentView>("/api/v1/finance/payments/"+payment.Id);
        Assert.Equal(payment.Id,read!.Id);Assert.Equal(10,read.Remaining);
        var outside=await Ok<PaymentView>(Post(a,"payments",new CreatePaymentRequest("economics",year,10,"Ngoài phạm vi")));
        using var scoped=await User("accounting","unit");
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync($"/api/v1/finance/adjustments?year={year}&departmentId=economics")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync("/api/v1/finance/payments/"+outside.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,(await a.GetAsync("/api/v1/finance/payments/missing-payment")).StatusCode);
    }
    [Fact]
    public async Task ScopedPeriodAndAdjustmentCannotAccessAnotherDepartment(){
        using var a=await User("accounting","unit");using var e=await User("executive","unit");
        Assert.Equal(HttpStatusCode.Forbidden,(await a.GetAsync("/api/v1/finance/period?departmentId=economics&year=2044")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await Post(e,"period",new ChangeFiscalPeriod("economics",2044,true,"Khóa"))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await Post(a,"adjustments",new CreateBudgetAdjustment("economics",2044,"general",10,"Thêm"))).StatusCode);
    }
}
