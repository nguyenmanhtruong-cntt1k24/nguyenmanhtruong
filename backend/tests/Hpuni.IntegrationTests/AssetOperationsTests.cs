using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class MaintenanceTests(SqlFactory factory):EquipmentScenario(factory),IClassFixture<SqlFactory>
{
    [Fact]
    public async Task PlannedServiceCompetesWithLoansAndCancelReleasesCapacity()
    {
        var(m,l,a)=await Setup(3);using var manager=m;using var lecturer=l;
        var job=await Ok<MaintenanceView>(Post(m,"maintenance",new PlanMaintenance(a.Id,"service",2,At(8),At(10),null)));
        var r=await Create(l,a,2);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(l,$"equipment-loans/{r.Id}/submit",new EquipmentCommand(r.Version,null))).StatusCode);
        await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/cancel",new FinishMaintenance(job.Version,"",0,null)));
        await Action(l,r,"submit");
        Assert.Equal(3,(await FreshAsset(m,a.Id)).AvailableNow);
    }
    [Fact]
    public async Task RunningServiceBlocksPastScheduledEndAndFinishReplayDoesNotDamageTwice()
    {
        var(m,l,a)=await Setup(3);using var manager=m;using var lecturer=l;
        var job=await Ok<MaintenanceView>(Post(m,"maintenance",new PlanMaintenance(a.Id,"service",2,DateTimeOffset.UtcNow.AddMinutes(-1),DateTimeOffset.UtcNow.AddHours(1),null)));
        job=await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/start",new FinishMaintenance(job.Version,"",0,null)));
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var row=await db.MaintenanceOrders.SingleAsync(x=>x.Id==job.Id);
            row.Start=DateTimeOffset.UtcNow.AddDays(-2);row.End=DateTimeOffset.UtcNow.AddDays(-1);await db.SaveChangesAsync();
        }
        job=(await m.GetFromJsonAsync<MaintenanceView>($"/api/v1/maintenance/{job.Id}"))!;
        var r=await Create(l,a,2);Assert.Equal(HttpStatusCode.Conflict,(await Post(l,$"equipment-loans/{r.Id}/submit",new EquipmentCommand(r.Version,null))).StatusCode);
        var key=Key();var input=new FinishMaintenance(job.Version,"unavailable",100000,"Cần sửa chữa");
        await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/finish",input,key));await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/finish",input,key));
        var after=await FreshAsset(m,a.Id);Assert.Equal(2,after.Damaged);Assert.Equal(1,after.AvailableNow);Assert.Equal(0,after.MaintenanceBlockedNow);
    }
    [Fact]
    public async Task RepairDoesNotReserveGoodStockAndOnlySuccessfulCompletionRestoresDamaged()
    {
        var(m,l,a)=await Setup(3);using var manager=m;using var lecturer=l;
        var r=await Borrow(m,l,a,2);
        r=await Ok<EquipmentLoanView>(Post(l,$"equipment-loans/{r.Id}/returns",new ReturnEquipment(r.Version,0,2,0,null)));
        await Ok<EquipmentLoanView>(Post(m,$"equipment-loans/{r.Id}/accept-return",new AcceptEquipmentReturn(r.Version,r.Returns.Single().Id,0,2,0,null)));
        var job=await Ok<MaintenanceView>(Post(m,"maintenance",new PlanMaintenance(a.Id,"repair",2,DateTimeOffset.UtcNow.AddMinutes(-1),DateTimeOffset.UtcNow.AddHours(1),null)));
        Assert.Equal(1,(await FreshAsset(m,a.Id)).AvailableNow);
        var competing=await Post(m,"maintenance",new PlanMaintenance(a.Id,"repair",1,At(8),At(10),null));Assert.Equal(HttpStatusCode.Conflict,competing.StatusCode);
        job=await Ok<MaintenanceView>(Post(m,$"maintenance/{job.Id}/start",new FinishMaintenance(job.Version,"",0,null)));
        var responses=await Task.WhenAll(Post(m,$"maintenance/{job.Id}/finish",new FinishMaintenance(job.Version,"good",0,null)),Post(m,$"maintenance/{job.Id}/finish",new FinishMaintenance(job.Version,"good",0,null)));
        Assert.Single(responses,x=>x.StatusCode==HttpStatusCode.OK);Assert.Single(responses,x=>x.StatusCode==HttpStatusCode.Conflict);
        var after=await FreshAsset(m,a.Id);Assert.Equal(0,after.Damaged);Assert.Equal(3,after.AvailableNow);
    }
}

[Collection("SQL Server")]
public class InventoryLogTests(SqlFactory factory):EquipmentScenario(factory),IClassFixture<SqlFactory>
{
    [Fact]
    public async Task InventorySnapshotIsStableAndCountingNeverMutatesStock()
    {
        var(m,l,a)=await Setup(5);using var manager=m;using var lecturer=l;
        var inv=await Ok<InventoryView>(Post(m,"inventories",new CreateInventory("Kiểm kê thử nghiệm","administration")));
        var row=Assert.Single(inv.Items,x=>x.AssetGroupId==a.Id);Assert.Equal(5,row.Expected);
        await Ok<AssetGroupView>(Post(m,$"equipment/{a.Id}/stock",new ReceiveStock(2,a.Version,null)));
        var incomplete=new SaveInventory(inv.Version,inv.Items.Select(x=>new CountInventoryItem(x.AssetGroupId,null,"",null)).ToArray(),true);
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(m,$"inventories/{inv.Id}/count",incomplete)).StatusCode);
        var input=incomplete with{Items=inv.Items.Select(x=>new CountInventoryItem(x.AssetGroupId,4,"missing",null)).ToArray()};
        var key=Key();var saved=await Ok<InventoryView>(Post(m,$"inventories/{inv.Id}/count",input,key));
        Assert.Equal("completed",saved.Status);Assert.Equal(5,Assert.Single(saved.Items,x=>x.AssetGroupId==a.Id).Expected);
        await Ok<InventoryView>(Post(m,$"inventories/{inv.Id}/count",input,key));
        Assert.Equal(7,(await FreshAsset(m,a.Id)).Total);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(m,$"inventories/{inv.Id}/count",input)).StatusCode);
    }
    [Fact]
    public async Task InventoryCountsHaveParentVersionAndRejectDuplicateRows()
    {
        var(m,l,a)=await Setup();using var manager=m;using var lecturer=l;
        var inv=await Ok<InventoryView>(Post(m,"inventories",new CreateInventory("Kiểm đếm","administration")));
        var rows=inv.Items.Select(x=>new CountInventoryItem(x.AssetGroupId,1,"good",null)).ToArray();
        // Ensure a true duplicate fixture even if only one group exists in this test host.
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(m,$"inventories/{inv.Id}/count",new SaveInventory(inv.Version,[rows[0],rows[0]],false))).StatusCode);
        var saved=await Ok<InventoryView>(Post(m,$"inventories/{inv.Id}/count",new SaveInventory(inv.Version,rows,false)));
        Assert.NotEqual(inv.Version,saved.Version);
        Assert.Equal(HttpStatusCode.Conflict,(await Post(m,$"inventories/{inv.Id}/count",new SaveInventory(inv.Version,rows,false))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await l.GetAsync($"/api/v1/inventories/{inv.Id}")).StatusCode);
        using var admin=await f.Login();var user=await SqlFactory.Create(admin,"resources","it");
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",new ConfigureScope("unit",[],[],user.Version)));
        using var scoped=await f.Login(user.UserName);
        var visible=await scoped.GetFromJsonAsync<JsonElement>("/api/v1/inventories");Assert.Equal(0,visible.GetProperty("total").GetInt32());
        Assert.Equal(HttpStatusCode.Forbidden,(await scoped.GetAsync($"/api/v1/inventories/{inv.Id}")).StatusCode);
    }
    [Fact]
    public async Task EquipmentLogReviewResubmitsSameRecordWithoutChangingInventory()
    {
        var(m,l,a)=await Setup(3);using var manager=m;using var lecturer=l;
        var r=await Borrow(m,l,a,2);
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(l,$"equipment-loans/{r.Id}/log",new SaveEquipmentLog(false,null,null,null,null,null,null))).StatusCode);
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var row=await db.EquipmentLoans.SingleAsync(x=>x.Id==r.Id);
            row.Start=DateTimeOffset.UtcNow.AddDays(-2);row.End=DateTimeOffset.UtcNow.AddDays(-1);row.HandedAt=row.Start;await db.SaveChangesAsync();
        }
        var log=await Ok<EquipmentLogView>(Post(l,$"equipment-loans/{r.Id}/log",new SaveEquipmentLog(false,null,null,null,null,null,null)));
        log=await Ok<EquipmentLogView>(Post(m,$"equipment-loans/{r.Id}/log/review",new ReviewRoomLog("needs_changes",null,log.Version)));
        var logId=log.Id;
        log=await Ok<EquipmentLogView>(Post(l,$"equipment-loans/{r.Id}/log",new SaveEquipmentLog(true,DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2),DateTimeOffset.UtcNow.AddDays(-1),2,"Tốt",null,log.Version)));
        Assert.Equal(logId,log.Id);
        log=await Ok<EquipmentLogView>(Post(m,$"equipment-loans/{r.Id}/log/review",new ReviewRoomLog("confirmed",null,log.Version)));
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(l,$"equipment-loans/{r.Id}/log",new SaveEquipmentLog(false,null,null,null,null,null,log.Version))).StatusCode);
        Assert.Equal(1,(await FreshAsset(m,a.Id)).AvailableNow);
        using var admin=await f.Login();var outsider=await SqlFactory.Create(admin);using var other=await f.Login(outsider.UserName);
        Assert.Equal(HttpStatusCode.NotFound,(await other.GetAsync($"/api/v1/equipment-loans/{r.Id}/log")).StatusCode);
    }
}
