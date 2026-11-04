using System.Net;
using System.Net.Http.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class NotificationsTests(FinanceFactory factory):IClassFixture<FinanceFactory>
{
    static string Key()=>Guid.NewGuid().ToString("N");
    async Task<(HttpClient Client,UserView User)> User(string role,string department="it",string scope="unit",string[]? denied=null)
    {
        var admin=await factory.Admin();var u=await SqlFactory.Create(admin,role,department);
        var response=await admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope(scope,[],denied??[],u.Version));
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);u=(await response.Content.ReadFromJsonAsync<UserView>())!;
        return(await factory.Login(u.UserName),u);
    }
    async Task<PaymentRequest> Payment(string department="it")
    {
        await using var scope=factory.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var payment=new PaymentRequest{DepartmentId=department,Year=2026,Amount=1,Content="Notification "+Key()};db.PaymentRequests.Add(payment);await db.SaveChangesAsync();return payment;
    }
    static async Task<NotificationPage> List(HttpClient client,string query="")
    {
        var response=await client.GetAsync("/api/v1/notifications?pageSize=100"+query);Assert.True(response.IsSuccessStatusCode,await response.Content.ReadAsStringAsync());
        return(await response.Content.ReadFromJsonAsync<NotificationPage>())!;
    }
    [Fact]
    public async Task ScopeAndDenialApplyBeforeCountsAndReadsDoNotLeakSources()
    {
        var (client,u)=await User("accounting");using var c=client;var own=await Payment();var other=await Payment("economics");
        var page=await List(c);Assert.Contains(page.Items,n=>n.SourceId==own.Id);Assert.DoesNotContain(page.Items,n=>n.SourceId==other.Id);
        var notice=page.Items.Single(n=>n.SourceId==own.Id);
        Assert.Equal(HttpStatusCode.NoContent,(await c.PostAsJsonAsync($"/api/v1/notifications/{notice.Id}/read",new{})).StatusCode);
        var admin=await factory.Admin();var fresh=await SqlFactory.Fresh(admin,u.Id);
        Assert.Equal(HttpStatusCode.OK,(await admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope("unit",[],["transactions.write"],fresh.Version))).StatusCode);
        var hidden=await List(c);Assert.Empty(hidden.Items);Assert.Equal(0,hidden.Total);Assert.Equal(0,hidden.Unread);
        Assert.Equal(HttpStatusCode.NotFound,(await c.PostAsJsonAsync($"/api/v1/notifications/{notice.Id}/read",new{})).StatusCode);
    }
    [Fact]
    public async Task ReadIsPerActorConcurrentAndNewSourceVersionBecomesUnread()
    {
        var (a,_)=await User("accounting");var (b,_)=await User("accounting");using var c=a;using var d=b;var source=await Payment();
        var notice=(await List(c)).Items.Single(n=>n.SourceId==source.Id);
        var responses=await Task.WhenAll(c.PostAsJsonAsync($"/api/v1/notifications/{notice.Id}/read",new{}),c.PostAsJsonAsync($"/api/v1/notifications/{notice.Id}/read",new{}));
        Assert.All(responses,r=>Assert.Equal(HttpStatusCode.NoContent,r.StatusCode));
        Assert.True((await List(c)).Items.Single(n=>n.SourceId==source.Id).Read);Assert.False((await List(d)).Items.Single(n=>n.SourceId==source.Id).Read);
        Assert.DoesNotContain((await List(c,"&unreadOnly=true")).Items,n=>n.SourceId==source.Id);
        await using(var scope=factory.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var payment=await db.PaymentRequests.SingleAsync(x=>x.Id==source.Id);payment.Content="Changed "+Key();payment.Status="part_paid";await db.SaveChangesAsync();
            Assert.Single(await db.Set<NotificationReadState>().Where(x=>x.SourceKey=="payment-pending:"+source.Id).ToArrayAsync());
        }
        var updated=(await List(c)).Items.Single(n=>n.SourceId==source.Id);Assert.NotEqual(notice.Id,updated.Id);Assert.False(updated.Read);
        Assert.Equal(HttpStatusCode.NotFound,(await c.PostAsJsonAsync($"/api/v1/notifications/{notice.Id}/read",new{})).StatusCode);
    }
    [Fact]
    public async Task ResourceOwnerScopeAndPersonalScopeAreIndependentForOverdueLoans()
    {
        var (manager,_)=await User("resources","economics");var (lecturer,owner)=await User("lecturer","it","self");var (outsider,_)=await User("lecturer","it","self");
        using var m=manager;using var l=lecturer;using var o=outsider;string loanId;
        await using(var scope=factory.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var a=new AssetGroup{Code="N-"+Key()[..12],Name="Scoped loan",DepartmentId="economics",Category="test",Unit="chiếc",Total=2};db.AssetGroups.Add(a);
            var loan=new EquipmentLoan{AssetGroupId=a.Id,DepartmentId="it",OwnerId=owner.Id,Quantity=2,Returned=1,Status="borrowed",Start=DateTimeOffset.UtcNow.AddDays(-3),End=DateTimeOffset.UtcNow.AddDays(-1),HandedAt=DateTimeOffset.UtcNow.AddDays(-3)};
            db.EquipmentLoans.Add(loan);await db.SaveChangesAsync();loanId=loan.Id;
        }
        var mine=(await List(l)).Items.Single(n=>n.SourceId==loanId);Assert.Equal("warning",mine.Severity);Assert.Contains((await List(m)).Items,n=>n.SourceId==loanId);Assert.DoesNotContain((await List(o)).Items,n=>n.SourceId==loanId);
        Assert.Equal(HttpStatusCode.NotFound,(await o.PostAsJsonAsync($"/api/v1/notifications/{mine.Id}/read",new{})).StatusCode);
        await using(var scope=factory.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var loan=await db.EquipmentLoans.SingleAsync(x=>x.Id==loanId);loan.Returned=2;loan.Status="completed";await db.SaveChangesAsync();
        }
        Assert.DoesNotContain((await List(l)).Items,n=>n.SourceId==loanId);Assert.DoesNotContain((await List(m)).Items,n=>n.SourceId==loanId);
    }
    [Fact]
    public async Task InboxIncludesScopedRoomBudgetAndMaintenanceTasksAndPaginatesDeterministically()
    {
        var (manager,owner)=await User("resources");var (executive,_)=await User("executive");using var m=manager;using var e=executive;
        string roomId,budgetId,jobId,adjustmentId;var now=DateTimeOffset.UtcNow;
        await using(var scope=factory.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var room=new Room{Code="NR-"+Key()[..10],Name="Notification room",Type="room",DepartmentId="it",Capacity=20};db.Rooms.Add(room);
            var request=new RoomRequest{RoomId=room.Id,DepartmentId="it",OwnerId=owner.Id,Attendees=1,Start=now.AddDays(1),End=now.AddDays(1).AddHours(1),Status="pending"};db.RoomRequests.Add(request);roomId=request.Id;
            var a=new AssetGroup{Code="NA-"+Key()[..10],Name="Notification asset",Category="test",Unit="chiếc",DepartmentId="it",Total=1};db.AssetGroups.Add(a);
            var job=new MaintenanceOrder{AssetGroupId=a.Id,DepartmentId="it",Quantity=1,Start=now.AddDays(-2),End=now.AddDays(-1),Status="running"};db.MaintenanceOrders.Add(job);jobId=job.Id;
            var proposal=new BudgetProposal{DepartmentId="it",Year=2026,Title="Notification budget",OwnerId=owner.Id,Amount=1,Status="pending"};db.BudgetProposals.Add(proposal);budgetId=proposal.Id;
            var adjustment=new BudgetAdjustment{DepartmentId="it",Year=2026,Category="general",Amount=1,Reason="Notification adjustment",ActorId=owner.Id};db.BudgetAdjustments.Add(adjustment);adjustmentId=adjustment.Id;await db.SaveChangesAsync();
        }
        var list=await List(m);Assert.Contains(list.Items,n=>n.SourceId==roomId);Assert.Contains(list.Items,n=>n.SourceId==jobId&&n.Severity=="warning");Assert.DoesNotContain(list.Items,n=>n.SourceId==budgetId);
        Assert.Contains((await List(e)).Items,n=>n.SourceId==budgetId);Assert.Contains((await List(e)).Items,n=>n.SourceId==adjustmentId);Assert.DoesNotContain((await List(e)).Items,n=>n.SourceId==roomId);
        var first=await m.GetFromJsonAsync<NotificationPage>("/api/v1/notifications?pageSize=1");var second=await m.GetFromJsonAsync<NotificationPage>("/api/v1/notifications?pageSize=1&page=2");Assert.NotEqual(first!.Items[0].Id,second!.Items[0].Id);
        Assert.Equal(HttpStatusCode.BadRequest,(await m.GetAsync("/api/v1/notifications?pageSize=101")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await m.PostAsJsonAsync("/api/v1/notifications/invalid/read",new{})).StatusCode);
        m.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");Assert.Equal(HttpStatusCode.BadRequest,(await m.PostAsJsonAsync($"/api/v1/notifications/{first.Items[0].Id}/read",new{})).StatusCode);
    }
    [Fact]
    public async Task UserWithoutRelevantPermissionsHasEmptyInboxAndAnonymousIsRejected()
    {
        var admin=await factory.Admin();var page=await List(admin);Assert.Empty(page.Items);Assert.Equal(0,page.Unread);
        using var anonymous=factory.Client();Assert.Equal(HttpStatusCode.Unauthorized,(await anonymous.GetAsync("/api/v1/notifications")).StatusCode);
    }
}
