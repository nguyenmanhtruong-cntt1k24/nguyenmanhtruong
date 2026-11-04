using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

public class FinanceFactory:SqlFactory
{
    private Task<HttpClient>? admin;
    public Task<HttpClient> Admin()=>admin??=Login();
}

[Collection("SQL Server")]
public class FinanceTests(FinanceFactory f):IClassFixture<FinanceFactory>
{
    static string Key()=>Guid.NewGuid().ToString();
    static Task<HttpResponseMessage> Post(HttpClient c,string path,object data,string? key=null){var request=new HttpRequestMessage(HttpMethod.Post,"/api/v1/"+path){Content=JsonContent.Create(data)};request.Headers.Add("Idempotency-Key",key??Key());return c.SendAsync(request);}
    static async Task<T> Ok<T>(Task<HttpResponseMessage> action){var r=await action;Assert.True(r.IsSuccessStatusCode,await r.Content.ReadAsStringAsync());return (await r.Content.ReadFromJsonAsync<T>())!;}
    async Task<HttpClient> User(string role,string scope="all",string department="it"){
        var admin=await f.Admin();var u=await SqlFactory.Create(admin,role,department);
        Assert.Equal(HttpStatusCode.OK,(await admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope(scope,[],[],u.Version))).StatusCode);
        return await f.Login(u.UserName);
    }
    static SaveProposal Draft(int year,decimal amount,string dep="it",string category="general")=>new(dep,year,category,"Dự toán kiểm thử",null,[new("Khoản mục",1,amount)]);
    static RecordFinanceEntry Entry(int year,decimal amount,string? voucher=null,string dep="it",string category="general")=>new(dep,year,category,amount,voucher??Key(),new DateOnly(year,1,2),"Chứng từ kiểm thử");
    async Task<ProposalView> Approve(HttpClient accounting,HttpClient executive,int year,decimal amount,string dep="it",string category="general"){
        var p=await Ok<ProposalView>(Post(accounting,"finance/proposals",Draft(year,amount,dep,category)));
        p=await Ok<ProposalView>(Post(accounting,$"finance/proposals/{p.Id}/submit",new ProposalCommand(p.Version)));
        return await Ok<ProposalView>(Post(executive,$"finance/proposals/{p.Id}/approve",new ProposalCommand(p.Version,"Đồng ý")));
    }
    [Fact]
    public async Task ApprovalReplayAllocatesOnceAndReceiptDoesNotIncreaseBudget(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2031;
        var p=await Ok<ProposalView>(Post(a,"finance/proposals",Draft(year,100)));
        var pending=await Ok<ProposalView>(Post(a,$"finance/proposals/{p.Id}/submit",new ProposalCommand(p.Version)));
        var key=Key();var cmd=new ProposalCommand(pending.Version,"Phê duyệt");
        await Ok<ProposalView>(Post(e,$"finance/proposals/{p.Id}/approve",cmd,key));await Ok<ProposalView>(Post(e,$"finance/proposals/{p.Id}/approve",cmd,key));
        Assert.Equal(HttpStatusCode.Conflict,(await Post(e,$"finance/proposals/{p.Id}/approve",cmd)).StatusCode);
        await Ok<FinanceEntryView>(Post(a,"finance/entries/receipt",Entry(year,900)));
        var b=await a.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}");Assert.Equal(100,b!.Allocated);Assert.Equal(100,b.Available);
        Assert.Equal(HttpStatusCode.Forbidden,(await Post(a,$"finance/proposals/{p.Id}/approve",cmd)).StatusCode);
    }
    [Fact]
    public async Task ConcurrentExpensesCannotOverspendAndFailureRollsBack(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2032;
        await Approve(a,e,year,100);var one=Entry(year,70);var two=Entry(year,70) with{DepartmentId="IT"};
        var results=await Task.WhenAll(Post(a,"finance/entries/expense",one),Post(a,"finance/entries/expense",two));
        Assert.Single(results,r=>r.StatusCode==HttpStatusCode.OK);Assert.Single(results,r=>r.StatusCode==HttpStatusCode.Conflict);
        var b=await a.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}");Assert.Equal(30,b!.Available);
        await using var scope=f.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();Assert.Single(await db.FinanceEntries.Where(x=>x.Year==year).ToArrayAsync());
        await Ok<FinanceEntryView>(Post(a,"finance/entries/expense",Entry(year,30))); // exactly remaining
    }
    [Fact]
    public async Task VoucherUniquenessSpansReceiptExpenseAndDepartment(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2033;await Approve(a,e,year,100);
        var voucher="Case-"+Key();await Ok<FinanceEntryView>(Post(a,"finance/entries/receipt",Entry(year,10,voucher)));
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"finance/entries/expense",Entry(year,10,voucher.ToLowerInvariant()))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"finance/entries/receipt",Entry(year,10,voucher,"economics"))).StatusCode);
    }
    [Fact]
    public async Task PaymentMustMatchSourceAndIsSpentOnlyOnce(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2034;await Approve(a,e,year,100);
        var p=await Ok<PaymentView>(Post(a,"finance/payments",new CreatePaymentRequest("it",year,60,"Chờ thanh toán")));
        var input=Entry(year,60) with{PaymentRequestId=p.Id,Version=p.Version};
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(a,"finance/entries/expense",input with{Amount=61})).StatusCode);
        var key=Key();await Ok<FinanceEntryView>(Post(a,"finance/entries/expense",input,key));await Ok<FinanceEntryView>(Post(a,"finance/entries/expense",input,key));
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"finance/entries/expense",input with{Voucher=Key()})).StatusCode);
        var b=await a.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}");Assert.Equal(40,b!.Available);
    }
    [Fact]
    public async Task ScopedFinanceCannotReadOrWriteOtherUnitOrBorrowAnotherRolesScope(){
        using var a=await User("accounting");using var e=await User("executive");var p=await Approve(a,e,2035,100,"economics");
        using var scoped=await User("accounting","unit");
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync($"/api/v1/finance/proposals/{p.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await Post(scoped,"finance/entries/receipt",Entry(2035,1,null,"economics"))).StatusCode);
        var list=await scoped.GetFromJsonAsync<JsonElement>("/api/v1/finance/proposals");Assert.DoesNotContain(list.GetProperty("items").EnumerateArray(),r=>r.GetProperty("departmentId").GetString()=="economics");
    }
    [Fact]
    public async Task InvalidAmountsDatesAndStaleDraftAreRejected(){
        using var a=await User("accounting");
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(a,"finance/proposals",Draft(2036,1.5m))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(a,"finance/entries/receipt",Entry(2036,10) with{Date=new DateOnly(2037,1,1)})).StatusCode);
        var p=await Ok<ProposalView>(Post(a,"finance/proposals",Draft(2036,100)));
        await Ok<ProposalView>(Post(a,$"finance/proposals/{p.Id}/submit",new ProposalCommand(p.Version)));
        using var req=new HttpRequestMessage(HttpMethod.Put,$"/api/v1/finance/proposals/{p.Id}"){Content=JsonContent.Create(Draft(2036,200) with{Version=p.Version})};req.Headers.Add("Idempotency-Key",Key());
        Assert.Equal(HttpStatusCode.Conflict,(await a.SendAsync(req)).StatusCode);
    }
    [Fact]
    public async Task MaintenanceFinishQueuesCostAtomicallyAndReplayDoesNotDuplicate(){
        using var m=await User("resources");using var a=await User("accounting");
        var asset=await Ok<AssetGroupView>(Post(m,"equipment",new SaveAssetGroup("FIN-"+Key()[..8],"Thiết bị bảo trì","Thiết bị","chiếc","it",true,null)));
        asset=await Ok<AssetGroupView>(Post(m,$"equipment/{asset.Id}/stock",new ReceiveStock(2,asset.Version,null)));
        var job=await Ok<MaintenanceView>(Post(m,"maintenance",new PlanMaintenance(asset.Id,"service",1,DateTimeOffset.UtcNow.AddMinutes(-1),DateTimeOffset.UtcNow.AddHours(1),"Bảo trì kiểm thử")));
        job=await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/start",new FinishMaintenance(job.Version,"",0,null)));
        var input=new FinishMaintenance(job.Version,"good",50,null);var key=Key();
        await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/finish",input,key));await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/finish",input,key));
        await using var scope=f.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var queue=Assert.Single(await db.PaymentRequests.Where(x=>x.MaintenanceId==job.Id).ToArrayAsync());Assert.Equal(50,queue.Amount);Assert.Equal("pending",queue.Status);
        Assert.False(await db.FinanceEntries.AnyAsync(x=>x.PaymentRequestId==queue.Id));
        Assert.Equal(HttpStatusCode.Forbidden,(await m.GetAsync("/api/v1/finance/payments")).StatusCode);
        var imported=await Ok<PaymentView>(Post(a,$"finance/payments/from-maintenance/{job.Id}",new ImportMaintenanceCost(queue.Year)));
        Assert.Equal(queue.Id,imported.Id);
        using var executive=await User("executive");await Approve(a,executive,queue.Year,50);
        var expense=await Ok<FinanceEntryView>(Post(a,"finance/entries/expense",Entry(queue.Year,50) with{PaymentRequestId=imported.Id,Version=imported.Version}));
        var detail=await m.GetFromJsonAsync<MaintenanceView>($"/api/v1/maintenance/{job.Id}");Assert.Equal("paid",detail!.PaymentStatus);Assert.Equal(expense.Voucher,detail.Voucher);Assert.Equal(expense.Id,detail.FinanceEntryId);
        var existing=await Ok<PaymentView>(Post(a,$"finance/payments/from-maintenance/{job.Id}",new ImportMaintenanceCost(queue.Year)));
        Assert.Equal(50,existing.Paid);Assert.Equal(0,existing.Remaining);
    }
    [Fact]
    public async Task HistoricalMaintenanceImportIsUniqueAndDuplicateVoucherCannotMarkRequestPaid(){
        using var a=await User("accounting");using var e=await User("executive");const int year=2038;await Approve(a,e,year,100);
        string id;
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var asset=new Hpuni.Domain.AssetGroup{Code="OLD-"+Key()[..8],Name="Tài sản lịch sử",DepartmentId="it",Category="Thiết bị",Unit="chiếc",Total=1};db.AssetGroups.Add(asset);
            var m=new Hpuni.Domain.MaintenanceOrder{AssetGroupId=asset.Id,DepartmentId="it",Status="completed",Result="good",Quantity=1,Start=DateTimeOffset.UtcNow.AddDays(-2),End=DateTimeOffset.UtcNow.AddDays(-1),Cost=40};db.MaintenanceOrders.Add(m);await db.SaveChangesAsync();id=m.Id;
        }
        var responses=await Task.WhenAll(Post(a,$"finance/payments/from-maintenance/{id}",new ImportMaintenanceCost(year)),Post(a,$"finance/payments/from-maintenance/{id}",new ImportMaintenanceCost(year)));
        var rows=await Task.WhenAll(responses.Select(async r=>{r.EnsureSuccessStatusCode();return (await r.Content.ReadFromJsonAsync<PaymentView>())!;}));Assert.Equal(rows[0].Id,rows[1].Id);
        var voucher=Key();await Ok<FinanceEntryView>(Post(a,"finance/entries/receipt",Entry(year,1,voucher)));
        var input=Entry(year,40,voucher) with{PaymentRequestId=rows[0].Id,Version=rows[0].Version};
        Assert.Equal(HttpStatusCode.Conflict,(await Post(a,"finance/entries/expense",input)).StatusCode);
        await using var check=f.Services.CreateAsyncScope();var dbCheck=check.ServiceProvider.GetRequiredService<HpuniDbContext>();Assert.Equal("pending",(await dbCheck.PaymentRequests.SingleAsync(p=>p.Id==rows[0].Id)).Status);
        Assert.False(await dbCheck.FinanceEntries.AnyAsync(x=>x.PaymentRequestId==rows[0].Id));
    }
}
