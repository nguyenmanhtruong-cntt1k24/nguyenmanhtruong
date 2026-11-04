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
public class LecturerWorkspaceTests(SqlFactory factory) : EquipmentScenario(factory), IClassFixture<SqlFactory>
{
    [Fact]
    public async Task WeeklyCalendarKeepsMoreThanOnePageAndSubmittedLogsKeepTheirReviewState()
    {
        using var admin=await f.Login();var user=await SqlFactory.Create(admin);using var owner=await f.Login(user.UserName);
        var label="Agenda"+Key()[..10];var start=DateTimeOffset.UtcNow.AddDays(-1);
        await using(var scope=f.Services.CreateAsyncScope())
        {
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var room=new Room{Code=label,Name=label,Type="Phòng học",DepartmentId="it",Capacity=60};db.Rooms.Add(room);
            for(var i=0;i<25;i++)
            {
                var request=new RoomRequest{RoomId=room.Id,OwnerId=user.Id,DepartmentId="it",Status="approved",Start=start,End=start.AddHours(1),Purpose=label,Attendees=5};db.RoomRequests.Add(request);
                if(i==0)db.RoomUseLogs.Add(new(){RequestId=request.Id,Used=false,Status="needs_changes",Issue="Buổi học hủy",ReviewNote="Bổ sung lý do"});
            }
            await db.SaveChangesAsync();
        }
        var date=start.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-dd");
        var calendar=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/calendar?date={date}");
        Assert.Equal(25,calendar.GetProperty("items").GetArrayLength());
        var logs=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/logs?search={label}");
        Assert.Equal(24,logs.GetProperty("eligibleTotal").GetInt32());Assert.Equal(1,logs.GetProperty("total").GetInt32());
        var log=Assert.Single(logs.GetProperty("items").EnumerateArray()).GetProperty("log");
        Assert.Equal("needs_changes",log.GetProperty("status").GetString());Assert.False(log.GetProperty("used").GetBoolean());
        Assert.Equal("Bổ sung lý do",log.GetProperty("reviewNote").GetString());
    }

    [Fact]
    public async Task DeviceDirectoryFiltersBeforePagingAndCannotFetchAnInactiveItemById()
    {
        using var admin=await f.Login();var user=await SqlFactory.Create(admin);using var owner=await f.Login(user.UserName);
        var label="Device"+Key()[..10];string inactiveId="";
        await using(var scope=f.Services.CreateAsyncScope())
        {
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            for(var i=0;i<21;i++)db.AssetGroups.Add(new(){Code=label+i.ToString("D2"),Name=label,Category=label,Unit="chiếc",DepartmentId="it",Total=3});
            var inactive=new AssetGroup{Code=label+"off",Name=label,Category=label,Unit="chiếc",DepartmentId="it",Total=3,Active=false};db.AssetGroups.Add(inactive);inactiveId=inactive.Id;
            await db.SaveChangesAsync();
        }
        var page=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/devices?category={label}&page=2&pageSize=20");
        Assert.Equal(21,page.GetProperty("total").GetInt32());Assert.Single(page.GetProperty("items").EnumerateArray());
        Assert.Contains(label,page.GetProperty("categories").EnumerateArray().Select(x=>x.GetString()));
        var inactiveResult=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/devices?assetGroupId={inactiveId}");
        Assert.Equal(0,inactiveResult.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task MixedRequestsPageInSqlAndSummaryCountsAllOwnedRowsWithinScope()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin); var other = await SqlFactory.Create(admin);
        using var owner = await f.Login(user.UserName);
        var label = "Lecturer" + Key()[..10];
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var room = new Room {Code=label,Name=label,Type="Phòng học",DepartmentId="it",Capacity=60};
            var asset = new AssetGroup {Code=label,Name=label,Category="Thiết bị",Unit="chiếc",DepartmentId="it",Total=100};
            db.Rooms.Add(room); db.AssetGroups.Add(asset);
            for (var i=0;i<45;i++)
            {
                if (i%2==0) db.RoomRequests.Add(new(){RoomId=room.Id,OwnerId=user.Id,DepartmentId="it",Status="pending",Start=At(8),End=At(10),Purpose=label,Attendees=5,CreatedAt=DateTimeOffset.UtcNow.AddSeconds(-i)});
                else db.EquipmentLoans.Add(new(){AssetGroupId=asset.Id,OwnerId=user.Id,DepartmentId="it",Status="draft",Start=At(8),End=At(10),Purpose=label,Quantity=1,CreatedAt=DateTimeOffset.UtcNow.AddSeconds(-i)});
            }
            db.RoomRequests.Add(new(){RoomId=room.Id,OwnerId=other.Id,DepartmentId="it",Status="pending",Start=At(8),End=At(10),Purpose=label,Attendees=5});
            await db.SaveChangesAsync();
        }
        var page = await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/requests?page=2&pageSize=20&search={label}");
        Assert.Equal(45,page.GetProperty("total").GetInt32());
        Assert.Equal(20,page.GetProperty("items").GetArrayLength());
        Assert.All(page.GetProperty("items").EnumerateArray(),x=>Assert.Equal(user.Id,x.GetProperty("request").GetProperty("ownerId").GetString()));
        Assert.Equal(23,page.GetProperty("counts").GetProperty("pending").GetInt32());
        var drafts = await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/requests?tab=draft&page=2&pageSize=20&search={label}");
        Assert.Equal(22,drafts.GetProperty("total").GetInt32());Assert.Equal(2,drafts.GetProperty("items").GetArrayLength());
        var summary = await owner.GetFromJsonAsync<JsonElement>("/api/v1/lecturer-workspace/summary");
        Assert.Equal(23,summary.GetProperty("pendingCount").GetInt32());Assert.Equal(4,summary.GetProperty("recent").GetArrayLength());
        var fresh = await SqlFactory.Fresh(admin,user.Id);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",new ConfigureScope("units",["economics"],[],fresh.Version)));
        var denied = await owner.GetFromJsonAsync<JsonElement>("/api/v1/lecturer-workspace/requests");
        Assert.Equal(0,denied.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task RoomFiltersExcludeCombinedFullDayBookingsBeforePaginationAndKeepTypesGlobal()
    {
        using var admin=await f.Login();var user=await SqlFactory.Create(admin);using var owner=await f.Login(user.UserName);
        var label="Rooms"+Key()[..10];string inactiveId="";var date=DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        await using(var scope=f.Services.CreateAsyncScope())
        {
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            for(var i=0;i<23;i++)
            {
                var room=new Room{Code=label+i.ToString("D2"),Name=label,Type=label,DepartmentId="it",Capacity=60};db.Rooms.Add(room);
                if(i<2) foreach(var span in new[]{(7,12),(12,18)}) db.RoomReservations.Add(new(){RoomId=room.Id,Start=new(date.ToDateTime(new TimeOnly(span.Item1,0)),TimeSpan.FromHours(7)),End=new(date.ToDateTime(new TimeOnly(span.Item2,0)),TimeSpan.FromHours(7)),Kind="maintenance",Active=true});
            }
            var inactive=new Room{Code=label+"off",Name=label,Type=label,DepartmentId="it",Capacity=60,Active=false};db.Rooms.Add(inactive);inactiveId=inactive.Id;
            await db.SaveChangesAsync();
        }
        var page=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/rooms?search={label}&type={label}&capacity=50&date={date:yyyy-MM-dd}&page=2&pageSize=20");
        Assert.Equal(21,page.GetProperty("total").GetInt32());Assert.Single(page.GetProperty("items").EnumerateArray());
        Assert.Contains(label,page.GetProperty("types").EnumerateArray().Select(x=>x.GetString()));
        var timed=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/rooms?search={label}&date={date:yyyy-MM-dd}&start=08:00&end=09:00");
        Assert.Equal(21,timed.GetProperty("total").GetInt32());
        var inactiveResult=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/rooms?roomId={inactiveId}");
        Assert.Equal(0,inactiveResult.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task ReadyReceiptsBeyondFiftyAndLogsHaveIndependentPagesAndOwnerFiltering()
    {
        using var admin=await f.Login();var user=await SqlFactory.Create(admin);using var owner=await f.Login(user.UserName);
        var label="Return"+Key()[..10];var ended=DateTimeOffset.UtcNow.AddDays(-2);
        await using(var scope=f.Services.CreateAsyncScope())
        {
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var asset=new AssetGroup{Code=label,Name=label,DepartmentId="it",Category="Thiết bị",Unit="chiếc",Total=100};db.AssetGroups.Add(asset);
            for(var i=0;i<55;i++)db.EquipmentLoans.Add(new(){AssetGroupId=asset.Id,OwnerId=user.Id,DepartmentId="it",Status="ready",HandedAt=ended,Start=At(8),End=At(10),Quantity=1,Purpose=label});
            for(var i=0;i<23;i++)db.EquipmentLoans.Add(new(){AssetGroupId=asset.Id,OwnerId=user.Id,DepartmentId="it",Status="borrowed",HandedAt=ended.AddHours(-1),Start=ended,End=ended.AddHours(2),Quantity=1,Purpose=label});
            db.EquipmentLoans.Add(new(){AssetGroupId=asset.Id,OwnerId=user.Id,DepartmentId="it",Status="borrowed",HandedAt=ended.AddHours(-1),Start=At(8),End=At(10),Quantity=1,Purpose=label});
            await db.SaveChangesAsync();
        }
        var returns=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/returns?search={label}&readyPage=3&pageSize=20");
        Assert.Equal(55,returns.GetProperty("readyTotal").GetInt32());Assert.Equal(15,returns.GetProperty("readyItems").GetArrayLength());Assert.Equal(24,returns.GetProperty("total").GetInt32());
        var logs=await owner.GetFromJsonAsync<JsonElement>($"/api/v1/lecturer-workspace/logs?search={label}&eligiblePage=2&pageSize=20");
        Assert.Equal(23,logs.GetProperty("eligibleTotal").GetInt32());Assert.Equal(3,logs.GetProperty("eligibleItems").GetArrayLength());
        Assert.Equal(HttpStatusCode.BadRequest,(await owner.GetAsync("/api/v1/lecturer-workspace/returns?readyPage=0")).StatusCode);
        var fresh=await SqlFactory.Fresh(admin,user.Id);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",new ConfigureScope("self",[],["logs.write"],fresh.Version)));
        Assert.Equal(HttpStatusCode.Forbidden,(await owner.GetAsync("/api/v1/lecturer-workspace/logs")).StatusCode);
    }
}
