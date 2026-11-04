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
public class AssetLifecycleTests(FinanceFactory factory) : EquipmentScenario(factory), IClassFixture<FinanceFactory>
{
    Task<HttpClient>? manager; Task<HttpClient>? executive;
    Task<HttpClient> Manager() => manager ??= User("resources");
    Task<HttpClient> Executive() => executive ??= User("executive");
    async Task<HttpClient> User(string role, string department = "it", bool combined = false)
    {
        var admin = await factory.Admin(); UserView user;
        if (combined)
        {
            var name = "lc." + Key()[..12]; user = await Ok<UserView>(admin.PostAsJsonAsync("/api/v1/users", new CreateUser(name, name + "@hpuni.test", "Lifecycle", department, SqlFactory.Password, ["resources", "executive"])));
        }
        else user = await SqlFactory.Create(admin, role, department);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope", new ConfigureScope("unit", [], [], user.Version)));
        return await factory.Login(user.UserName);
    }
    async Task<AssetGroup> Asset(int total = 5, int damaged = 0, int lost = 0, string department = "it")
    {
        await Manager(); await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var a = new AssetGroup { Code = "LC-" + Key()[..10], Name = "Tài sản vòng đời", DepartmentId = department, Unit = "chiếc", Category = "Thiết bị", Total = total, Damaged = damaged, Lost = lost }; db.AssetGroups.Add(a); await db.SaveChangesAsync(); return a;
    }
    static CreateAssetDisposal Request(AssetGroup a, int good = 1, int damaged = 0) => new(a.Id, good, damaged, "Thanh lý theo hồ sơ", "TL-" + Key());
    static Task<AssetDisposalView> CreateDisposal(HttpClient c, CreateAssetDisposal input) => Ok<AssetDisposalView>(Post(c, "asset-lifecycle/disposals", input));
    static Task<HttpResponseMessage> Review(HttpClient c, AssetDisposalView d, string operation = "approve", string? key = null) => Post(c, $"asset-lifecycle/disposals/{d.Id}/{operation}", new ReviewAssetDisposal(d.Version, "Quyết định xử lý"), key);
    [Fact]
    public async Task ProfileReplaysUpdatesVersionAndDoesNotChangeStockOrInventValue()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset();
        var input = new SaveFixedAssetProfile(20000000, new(2020, 1, 1), "Mua sắm", "HD-001", "Nguyên giá cả nhóm"); var key = Key();
        var first = await Ok<FixedAssetProfileView>(Post(m, "asset-lifecycle/profiles/" + a.Id.ToUpperInvariant(), input, key));
        Assert.Equal(a.Id, first.AssetGroupId);
        Assert.Equal(first, await Ok<FixedAssetProfileView>(Post(m, "asset-lifecycle/profiles/" + a.Id.ToUpperInvariant(), input, key)));
        Assert.Equal(HttpStatusCode.Conflict, (await Post(m, "asset-lifecycle/profiles/" + a.Id, input)).StatusCode);
        var updated = await Ok<FixedAssetProfileView>(Post(m, "asset-lifecycle/profiles/" + a.Id, input with { AcquisitionValue = 21000000, Version = first.Version })); Assert.NotEqual(first.Version, updated.Version);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(m, "asset-lifecycle/profiles/" + a.Id, input with { Version = first.Version })).StatusCode);
        Assert.Equal(21000000, (await e.GetFromJsonAsync<FixedAssetProfileView>("/api/v1/asset-lifecycle/profiles/" + a.Id))!.AcquisitionValue);
        Assert.Equal(HttpStatusCode.Forbidden, (await Post(e, "asset-lifecycle/profiles/" + a.Id, input with { Version = updated.Version })).StatusCode);
        var report = (await m.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/assets?year=2026"))!; var row = Assert.Single(report.Items, r => r.Id == a.Id); Assert.Equal("21000000", row.Cells[9]); Assert.Equal("2020-01-01", row.Cells[10]);
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); Assert.Equal(5, (await db.AssetGroups.FindAsync(a.Id))!.Total); Assert.Equal(2, await db.AuditEvents.CountAsync(x => x.TargetId == a.Id && x.Action == "asset-profile.save"));
    }
    [Fact]
    public async Task ApprovedDisposalReducesCorrectPoolsExactlyOnceAndKeepsHistoricalProfile()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset(8, 3, 1);
        await Ok<FixedAssetProfileView>(Post(m, "asset-lifecycle/profiles/" + a.Id, new SaveFixedAssetProfile(40000000, new(2020, 1, 1), "Ngân sách", "HD-002", null)));
        var input = Request(a, 2, 2); var creationKey = Key(); var d = await Ok<AssetDisposalView>(Post(m, "asset-lifecycle/disposals", input, creationKey));
        Assert.Equal(d.Id, (await Ok<AssetDisposalView>(Post(m, "asset-lifecycle/disposals", input, creationKey))).Id);
        var pendingNotifications = (await e.GetFromJsonAsync<NotificationPage>("/api/v1/notifications?pageSize=100"))!;
        Assert.Contains(pendingNotifications.Items, n => n.Kind == "asset-disposal" && n.SourceId == d.Id && n.Url.Contains("executive.html"));
        await using (var scope = f.Services.CreateAsyncScope()) Assert.Equal(8, (await scope.ServiceProvider.GetRequiredService<HpuniDbContext>().AssetGroups.FindAsync(a.Id))!.Total);
        var key = Key(); var approved = await Ok<AssetDisposalView>(Review(e, d, key: key)); Assert.Equal("approved", approved.Status); Assert.Equal(approved, await Ok<AssetDisposalView>(Review(e, d, key: key)));
        Assert.Equal(HttpStatusCode.Conflict, (await Review(e, d)).StatusCode);
        var resolvedNotifications = (await e.GetFromJsonAsync<NotificationPage>("/api/v1/notifications?pageSize=100"))!; Assert.DoesNotContain(resolvedNotifications.Items, n => n.Kind == "asset-disposal" && n.SourceId == d.Id);
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); var current = (await db.AssetGroups.FindAsync(a.Id))!; Assert.Equal(4, current.Total); Assert.Equal(1, current.Damaged); Assert.Equal(1, current.Lost); Assert.True(current.Active);
            var moves = await db.EquipmentMovements.Where(x => x.AssetGroupId == a.Id).ToArrayAsync(); Assert.Equal(2, moves.Length); Assert.Equal(-4, moves.Sum(x => x.Quantity)); Assert.All(moves, x => Assert.Equal(d.Id, x.Note));
            Assert.Equal(40000000, (await db.Set<FixedAssetProfile>().FindAsync(a.Id))!.AcquisitionValue); Assert.Equal(1, await db.AuditEvents.CountAsync(x => x.TargetId == d.Id && x.Action == "asset-disposal.approve"));
        }
    }
    [Fact]
    public async Task ApprovalRechecksFutureReservationsAndDoesNotMutateOnFailure()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset(3); var d = await CreateDisposal(m, Request(a, 2));
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); var actor = await db.Users.Where(x => x.UserName == "admin.hpuni").Select(x => x.Id).SingleAsync();
            db.EquipmentLoans.Add(new() { AssetGroupId = a.Id, DepartmentId = "economics", OwnerId = actor, Quantity = 2, Start = DateTimeOffset.UtcNow.AddYears(2), End = DateTimeOffset.UtcNow.AddYears(2).AddHours(1), Status = "pending" }); await db.SaveChangesAsync();
        }
        var failed = await Review(e, d); Assert.Equal(HttpStatusCode.Conflict, failed.StatusCode); Assert.Contains("DISPOSAL_RESERVED", await failed.Content.ReadAsStringAsync());
        await using var check = f.Services.CreateAsyncScope(); var current = check.ServiceProvider.GetRequiredService<HpuniDbContext>(); Assert.Equal(3, (await current.AssetGroups.FindAsync(a.Id))!.Total); Assert.Empty(await current.EquipmentMovements.Where(x => x.AssetGroupId == a.Id).ToArrayAsync()); Assert.Equal("pending", (await current.Set<AssetDisposal>().FindAsync(d.Id))!.Status);
    }
    [Fact]
    public async Task DamagedPoolRespectsRepairAndRejectDoesNotChangeInventory()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset(6, 3, 1); var d = await CreateDisposal(m, Request(a, 0, 2));
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); db.MaintenanceOrders.Add(new() { AssetGroupId = a.Id, DepartmentId = a.DepartmentId, Mode = "repair", Quantity = 2, Start = DateTimeOffset.UtcNow.AddDays(1), End = DateTimeOffset.UtcNow.AddDays(2), Status = "planned" }); await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Conflict, (await Review(e, d)).StatusCode); var rejected = await Ok<AssetDisposalView>(Review(e, d, "reject")); Assert.Equal("rejected", rejected.Status);
        var allowed = await CreateDisposal(m, Request(a, 0, 1)); await Ok<AssetDisposalView>(Review(e, allowed));
        await using var check = f.Services.CreateAsyncScope(); var dbcheck = check.ServiceProvider.GetRequiredService<HpuniDbContext>(); var current = (await dbcheck.AssetGroups.FindAsync(a.Id))!; Assert.Equal(5, current.Total); Assert.Equal(2, current.Damaged); Assert.Equal(1, current.Lost);
    }
    [Fact]
    public async Task ConcurrentApprovalsCannotDisposeTheSameCapacity()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset(3); var one = await CreateDisposal(m, Request(a, 2)); var two = await CreateDisposal(m, Request(a, 2));
        var results = await Task.WhenAll(Review(e, one), Review(e, two)); Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK); Assert.Single(results, x => x.StatusCode == HttpStatusCode.Conflict);
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); Assert.Equal(1, (await db.AssetGroups.FindAsync(a.Id))!.Total); Assert.Single(await db.EquipmentMovements.Where(x => x.AssetGroupId == a.Id).ToArrayAsync());
    }
    [Fact]
    public async Task ScopeAndSeparationOfDutiesAreEnforcedForDirectoryProfileAndDisposal()
    {
        var m = await Manager(); var e = await Executive(); var a = await Asset(); var outside = await Asset(department: "economics"); var d = await CreateDisposal(m, Request(a));
        Assert.Equal(HttpStatusCode.Forbidden, (await m.GetAsync("/api/v1/asset-lifecycle/profiles/" + outside.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Post(e, "asset-lifecycle/disposals", Request(a))).StatusCode); Assert.Equal(HttpStatusCode.Forbidden, (await Review(m, d)).StatusCode);
        using var outsider = await User("executive", "economics"); Assert.Equal(HttpStatusCode.Forbidden, (await Review(outsider, d)).StatusCode); Assert.Equal(HttpStatusCode.Forbidden, (await outsider.GetAsync("/api/v1/asset-lifecycle/disposals/" + d.Id)).StatusCode);
        var list = await e.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/v1/asset-lifecycle/assets?pageSize=100"); Assert.DoesNotContain(list.GetProperty("items").EnumerateArray(), x => x.GetProperty("id").GetString() == outside.Id);
        using var combined = await User("resources", combined: true); var own = await CreateDisposal(combined, Request(a)); Assert.Equal(HttpStatusCode.BadRequest, (await Review(combined, own)).StatusCode);
    }
    [Fact]
    public async Task InvalidQuantitiesDatesAndMissingCsrfAreRejected()
    {
        var m = await Manager(); var a = await Asset(2, 1, 1);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, "asset-lifecycle/disposals", Request(a, 0, 0))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, "asset-lifecycle/disposals", Request(a, -1, 1))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(m, "asset-lifecycle/disposals", Request(a, 1, 0))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, "asset-lifecycle/profiles/" + a.Id, new SaveFixedAssetProfile(1.5m, new(2020, 1, 1), "Nguồn", "HD", null))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, "asset-lifecycle/profiles/" + a.Id, new SaveFixedAssetProfile(1, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)), "Nguồn", "HD", null))).StatusCode);
        using var noCsrf = await User("resources"); noCsrf.DefaultRequestHeaders.Remove("X-CSRF-TOKEN"); Assert.Equal(HttpStatusCode.BadRequest, (await Post(noCsrf, "asset-lifecycle/disposals", Request(a, 0, 1))).StatusCode);
    }
    [Fact]
    public async Task DisposalQueueSearchAndStatusWorkAcrossAssetsWithinPermissionScope()
    {
        var m = await Manager(); var e = await Executive(); var first = await Asset(); var second = await Asset(); var tag = "FILTER-" + Key();
        var one = await CreateDisposal(m, Request(first) with { Reference = tag + "-P" }); var two = await CreateDisposal(m, Request(second) with { Reference = tag + "-A" }); await Ok<AssetDisposalView>(Review(e, two));
        var all = await e.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/v1/asset-lifecycle/disposals?search=" + tag); Assert.Equal(2, all.GetProperty("total").GetInt32());
        Assert.All(all.GetProperty("items").EnumerateArray(), row => { Assert.StartsWith("LC-", row.GetProperty("assetCode").GetString()); Assert.Equal("Tài sản vòng đời", row.GetProperty("assetName").GetString()); });
        var pending = await e.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/v1/asset-lifecycle/disposals?status=pending&search=" + tag); Assert.Equal(one.Id, Assert.Single(pending.GetProperty("items").EnumerateArray()).GetProperty("id").GetString());
        var byCode = await e.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/v1/asset-lifecycle/disposals?search=" + second.Code); Assert.Equal(two.Id, Assert.Single(byCode.GetProperty("items").EnumerateArray()).GetProperty("id").GetString());
        using var outsider = await User("executive", "economics"); var outside = await outsider.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/v1/asset-lifecycle/disposals?search=" + tag); Assert.Equal(0, outside.GetProperty("total").GetInt32());
        Assert.Equal(HttpStatusCode.BadRequest, (await e.GetAsync("/api/v1/asset-lifecycle/disposals?status=deleted")).StatusCode);
    }
}
