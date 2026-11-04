using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class FrontendFollowupTests(SqlFactory factory) : EquipmentScenario(factory), IClassFixture<SqlFactory>
{
    [Fact]
    public async Task ResourcePagesFilterMixedRequestsAndLazyInventoryWithinScope()
    {
        using var admin=await f.Login();var u=await SqlFactory.Create(admin,"resources","it");
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope("unit",[],[],u.Version)));
        var owner=await SqlFactory.Create(admin);var label="Page"+Key()[..10];
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            foreach(var dep in new[]{"it","economics"}){
                var a=new AssetGroup{Code=label+dep,Name=label,Category="Test",Unit="unit",DepartmentId=dep,Total=3};db.AssetGroups.Add(a);
                var r=new Room{Code=label+dep,Name=label,Type="Test",DepartmentId=dep,Capacity=5};db.Rooms.Add(r);
                foreach(var state in new[]{"pending","draft"}){
                    db.RoomRequests.Add(new(){RoomId=r.Id,DepartmentId=dep,OwnerId=owner.Id,Status=state,Start=At(8),End=At(9),Attendees=1});
                    db.EquipmentLoans.Add(new(){AssetGroupId=a.Id,DepartmentId=dep,OwnerId=owner.Id,Status=state,Start=At(8),End=At(9),Quantity=1});
                }
                if(dep=="it")db.EquipmentLoans.Add(new(){AssetGroupId=a.Id,DepartmentId="economics",OwnerId=owner.Id,Status="approved",Start=At(10),End=At(11),Quantity=1});
                db.InventorySessions.Add(new(){Name=label,DepartmentId=dep});
                db.MaintenanceOrders.Add(new(){AssetGroupId=a.Id,DepartmentId=dep,Quantity=1,Start=At(8),End=At(9),Content=label});
            }
            await db.SaveChangesAsync();
        }
        using var c=await f.Login(u.UserName);
        var requests=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/requests?search={label}&status=pending&page=2&pageSize=1");
        Assert.Equal(2,requests.GetProperty("total").GetInt32());Assert.Equal("it",Assert.Single(requests.GetProperty("items").EnumerateArray()).GetProperty("request").GetProperty("departmentId").GetString());
        var loans=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/requests?search={label}&kind=device&status=pending");Assert.Equal(1,loans.GetProperty("total").GetInt32());
        var cross=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/requests?search={label}&kind=device&status=approved");
        var crossRow=Assert.Single(cross.GetProperty("items").EnumerateArray());Assert.Equal("it",crossRow.GetProperty("manageDepartmentId").GetString());Assert.Equal("economics",crossRow.GetProperty("request").GetProperty("departmentId").GetString());
        var inventories=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/inventories?search={label}");Assert.Equal(1,inventories.GetProperty("total").GetInt32());Assert.Equal(0,Assert.Single(inventories.GetProperty("items").EnumerateArray()).GetProperty("itemCount").GetInt32());
        var maintenance=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/maintenance?search={label}");Assert.Equal(1,maintenance.GetProperty("total").GetInt32());
        Assert.Equal(HttpStatusCode.BadRequest,(await c.GetAsync("/api/v1/resource-workspace/requests?pageSize=101")).StatusCode);
    }
    [Fact]
    public async Task AdminSearchAndFiltersRunBeforePagingAndRespectScope()
    {
        using var admin=await f.Login();var label="Filter"+Key()[..10];
        foreach(var dep in new[]{"it","it","economics"}){
            var u=await SqlFactory.Create(admin,"lecturer",dep);
            await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{u.Id}",new UpdateUser(u.Email,label,dep,["lecturer"],u.Version)));
        }
        var result=await admin.GetFromJsonAsync<JsonElement>($"/api/v1/users?search={label}&departmentId=it&status=active&roleId=lecturer&page=2&pageSize=1&includeSummary=true");
        Assert.Equal(2,result.GetProperty("total").GetInt32());Assert.Single(result.GetProperty("items").EnumerateArray());Assert.True(result.GetProperty("summary").GetProperty("total").GetInt32()>=3);
        var limited=await SqlFactory.Create(admin,"resources","it");
        limited=await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{limited.Id}/scope",new ConfigureScope("unit",[],[],limited.Version)));
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{limited.Id}/role-overrides",new ConfigureOverride("resources",false,"unit",["users.manage"],limited.Version)));
        using var c=await f.Login(limited.UserName);
        var outside=await c.GetFromJsonAsync<JsonElement>($"/api/v1/users?search={label}&departmentId=economics");Assert.Equal(0,outside.GetProperty("total").GetInt32());
    }
    [Fact]
    public async Task ManageDirectoryIncludesInactiveOnlyWithinManageScope()
    {
        using var admin=await f.Login();
        var u=await SqlFactory.Create(admin,"resources","it");
        u=await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope("unit",[],[],u.Version)));
        using var c=await f.Login(u.UserName);
        var code=Key()[..10];
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            foreach(var dep in new[]{"it","economics"}){
                db.AssetGroups.Add(new(){Code="A"+code+dep,Name="Inactive",Category="Test",Unit="unit",DepartmentId=dep,Active=false});
                db.Rooms.Add(new(){Code="R"+code+dep,Name="Inactive",Type="Test",Capacity=10,DepartmentId=dep,Active=false});
            }
            await db.SaveChangesAsync();
        }
        foreach(var path in new[]{"equipment","rooms"}){
            var visible=await c.GetFromJsonAsync<JsonElement>($"/api/v1/{path}?manage=true&search={code}");
            var row=Assert.Single(visible.GetProperty("items").EnumerateArray());
            Assert.False(row.GetProperty("active").GetBoolean());Assert.Equal("it",row.GetProperty("departmentId").GetString());
            var publicRows=await c.GetFromJsonAsync<JsonElement>($"/api/v1/{path}?search={code}");Assert.Empty(publicRows.GetProperty("items").EnumerateArray());
            var workspace=await c.GetFromJsonAsync<JsonElement>($"/api/v1/resource-workspace/{(path=="equipment"?"assets":"rooms")}?search={code}&pageSize=1");
            Assert.Equal(1,workspace.GetProperty("total").GetInt32());Assert.Equal("it",Assert.Single(workspace.GetProperty("items").EnumerateArray()).GetProperty("departmentId").GetString());
        }
        using var lecturer=await f.Login((await SqlFactory.Create(admin)).UserName);
        Assert.Equal(HttpStatusCode.Forbidden,(await lecturer.GetAsync("/api/v1/equipment?manage=true")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await lecturer.GetAsync("/api/v1/rooms?manage=true")).StatusCode);
    }

    [Fact]
    public async Task LogReviewMetadataUsesItsOwnPermissionAndScope()
    {
        using var admin=await f.Login();
        var actor=await SqlFactory.Create(admin,"resources","it");
        actor=await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{actor.Id}/scope",new ConfigureScope("unit",[],[],actor.Version)));
        actor=await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{actor.Id}/role-overrides",new ConfigureOverride("resources",false,"unit",["logs.review"],actor.Version)));
        var owner=await SqlFactory.Create(admin);
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            foreach(var dep in new[]{"it","economics"}){
                var a=new AssetGroup{Code=Key()[..12],Name="Log asset",Category="Test",Unit="unit",DepartmentId=dep,Total=1};db.AssetGroups.Add(a);
                var r=new Room{Code=Key()[..12],Name="Log room",DepartmentId=dep,Type="Test",Capacity=10};db.Rooms.Add(r);
                foreach(var state in new[]{"approved","draft"}) {var request=new RoomRequest{RoomId=r.Id,DepartmentId=dep,OwnerId=owner.Id,Status=state,Start=At(8),End=At(9),Attendees=1};db.RoomRequests.Add(request);if(state=="approved")db.RoomUseLogs.Add(new(){RequestId=request.Id,Used=false,Status="submitted"});}
                foreach(var state in new[]{"borrowed","draft"}) {var loan=new EquipmentLoan{AssetGroupId=a.Id,DepartmentId=dep,OwnerId=owner.Id,Status=state,Start=At(8),End=At(9),Quantity=1,HandedAt=state=="borrowed"?DateTimeOffset.UtcNow:null};db.EquipmentLoans.Add(loan);if(state=="borrowed")db.EquipmentUseLogs.Add(new(){LoanId=loan.Id,Used=false,Status="submitted"});}
            }
            await db.SaveChangesAsync();
        }
        using var c=await f.Login(actor.UserName);
        foreach(var path in new[]{"room-requests","equipment-loans"}){
            var page=await c.GetFromJsonAsync<JsonElement>($"/api/v1/{path}?logReview=true&pageSize=1");
            Assert.Equal(1,page.GetProperty("total").GetInt32());
            var row=Assert.Single(page.GetProperty("items").EnumerateArray());Assert.Equal("it",row.GetProperty("departmentId").GetString());Assert.NotEqual("draft",row.GetProperty("status").GetString());
            Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync($"/api/v1/{path}?review=true")).StatusCode);
        }
        using var l=await f.Login(owner.UserName);
        Assert.Equal(HttpStatusCode.Forbidden,(await l.GetAsync("/api/v1/room-requests?logReview=true")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await l.GetAsync("/api/v1/equipment-loans?logReview=true")).StatusCode);
        var logs=await c.GetFromJsonAsync<JsonElement>("/api/v1/resource-workspace/logs?page=2&pageSize=1&status=pending");
        Assert.Equal(2,logs.GetProperty("total").GetInt32());Assert.Equal("it",Assert.Single(logs.GetProperty("items").EnumerateArray()).GetProperty("request").GetProperty("departmentId").GetString());
        var summary=await c.GetFromJsonAsync<JsonElement>("/api/v1/resource-workspace/summary");Assert.Equal(2,summary.GetProperty("pendingLogs").GetInt32());Assert.Equal(JsonValueKind.Null,summary.GetProperty("owned").ValueKind);
        Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/v1/resource-workspace/requests")).StatusCode);
    }
}
