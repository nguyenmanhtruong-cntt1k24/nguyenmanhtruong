using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

public abstract class EquipmentScenario(SqlFactory factory)
{
    protected SqlFactory f => factory;
    protected static string Key() => Guid.NewGuid().ToString("N");
    protected static DateTimeOffset At(int hour) => new(DateTime.Today.AddDays(3).AddHours(hour), TimeSpan.FromHours(7));
    protected static Task<HttpResponseMessage> Post(HttpClient c, string path, object body, string? key = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/" + path) { Content = JsonContent.Create(body) };
        request.Headers.Add("Idempotency-Key", key ?? Key()); return c.SendAsync(request);
    }
    protected static async Task<T> Ok<T>(Task<HttpResponseMessage> pending)
    {
        var response = await pending;
        Assert.True(response.StatusCode == HttpStatusCode.OK, response.StatusCode + ": " + await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }
    protected async Task<(HttpClient Manager, HttpClient Lecturer, AssetGroupView Asset)> Setup(int total = 5)
    {
        using var admin = await f.Login();
        var m = await SqlFactory.Create(admin, "resources", "administration");
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{m.Id}/scope", new ConfigureScope("all", [], [], m.Version)));
        var l = await SqlFactory.Create(admin);
        var manager = await f.Login(m.UserName); var lecturer = await f.Login(l.UserName);
        var a = await Ok<AssetGroupView>(Post(manager, "equipment", new SaveAssetGroup("TB" + Key()[..10], "Máy chiếu", "Giảng dạy", "chiếc", "administration", true, null)));
        a = await Ok<AssetGroupView>(Post(manager, $"equipment/{a.Id}/stock", new ReceiveStock(total, a.Version, null)));
        return (manager, lecturer, a);
    }
    protected static Task<EquipmentLoanView> Create(HttpClient l, AssetGroupView a, int quantity, int start = 8, int end = 10) =>
        Ok<EquipmentLoanView>(Post(l, "equipment-loans", new SaveEquipmentLoan(a.Id, At(start), At(end), quantity, null, null)));
    protected static Task<EquipmentLoanView> Action(HttpClient c, EquipmentLoanView r, string op, int? quantity = null) =>
        Ok<EquipmentLoanView>(Post(c, $"equipment-loans/{r.Id}/{op}", new EquipmentCommand(r.Version, null, quantity)));
    protected static async Task<EquipmentLoanView> Borrow(HttpClient m, HttpClient l, AssetGroupView a, int quantity)
    {
        var r = await Create(l, a, quantity); r = await Action(l, r, "submit");
        r = await Action(m, r, "approve"); r = await Action(m, r, "handover", quantity); return await Action(l, r, "receipt", quantity);
    }
    protected static async Task<AssetGroupView> FreshAsset(HttpClient c, string id)
    {
        var result = await c.GetFromJsonAsync<JsonElement>("/api/v1/equipment?pageSize=100");
        return result.GetProperty("items").EnumerateArray().Select(x => x.Deserialize<AssetGroupView>(new JsonSerializerOptions(JsonSerializerDefaults.Web))!).Single(x => x.Id == id);
    }
}

[Collection("SQL Server")]
public class EquipmentTests(SqlFactory factory) : EquipmentScenario(factory), IClassFixture<SqlFactory>
{
    [Fact]
    public async Task StockReceiptsReplayWithoutDoubleCountingAndEnforceVersion()
    {
        var (m, l, a) = await Setup(); using var manager = m; using var lecturer = l;
        var key = Key(); var payload = new ReceiveStock(2, a.Version, "Nhập thêm");
        var first = await Ok<AssetGroupView>(Post(m, $"equipment/{a.Id}/stock", payload, key));
        var again = await Ok<AssetGroupView>(Post(m, $"equipment/{a.Id}/stock", payload, key));
        Assert.Equal(7, first.Total); Assert.Equal(first, again);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(m, $"equipment/{a.Id}/stock", payload)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(m, $"equipment/{a.Id}/stock", payload with { Quantity = 3 }, key)).StatusCode);
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(2, await db.EquipmentMovements.CountAsync(x => x.AssetGroupId == a.Id));
        Assert.Equal(2, await db.AuditEvents.CountAsync(x => x.TargetId == a.Id && x.Action == "equipment.stock"));
    }
    [Fact]
    public async Task TwelveConcurrentSubmitsCannotOverbookOneUnit()
    {
        var (m, l, a) = await Setup(1); using var manager = m; using var lecturer = l;
        var drafts = new List<EquipmentLoanView>(); for (var i = 0; i < 12; i++) drafts.Add(await Create(l, a, 1));
        var results = await Task.WhenAll(drafts.Select(r => Post(l, $"equipment-loans/{r.Id}/submit", new EquipmentCommand(r.Version, null))));
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Equal(11, results.Count(x => x.StatusCode == HttpStatusCode.Conflict));
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(1, await db.EquipmentLoans.CountAsync(x => x.AssetGroupId == a.Id && x.Status == "pending"));
    }
    [Fact]
    public async Task CalendarUsesPeakAndCancelReleasesOnlyItsOwnQuantity()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        var r1 = await Action(l, await Create(l, a, 3, 8, 10), "submit");
        await Action(l, await Create(l, a, 3, 10, 12), "submit");
        var r3 = await Action(l, await Create(l, a, 2, 8, 12), "submit");
        var excess = await Create(l, a, 1, 8, 12);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(l, $"equipment-loans/{excess.Id}/submit", new EquipmentCommand(excess.Version, null))).StatusCode);
        await Action(l, r1, "cancel");
        Assert.Equal(HttpStatusCode.Conflict, (await Post(l, $"equipment-loans/{excess.Id}/submit", new EquipmentCommand(excess.Version, null))).StatusCode);
        await Action(l, r3, "cancel"); await Action(l, excess, "submit");
    }
    [Fact]
    public async Task HandoverReceiptDisputeRequiresOwnerConfirmationAndKeepsAllStockOccupied()
    {
        var (m, l, a) = await Setup(3); using var manager = m; using var lecturer = l;
        var r = await Action(l, await Create(l, a, 3), "submit"); r = await Action(m, r, "approve");
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, $"equipment-loans/{r.Id}/handover", new EquipmentCommand(r.Version, null, 2))).StatusCode);
        r = await Action(m, r, "handover", 3); r = await Action(l, r, "receipt", 2);
        Assert.Equal("receipt_disputed", r.Status); Assert.Equal(0, (await FreshAsset(m, a.Id)).AvailableNow);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, 1, 0, 0, null))).StatusCode);
        r = await Action(m, r, "resolve-receipt"); Assert.Equal("ready", r.Status);
        r = await Action(l, r, "receipt", 3); Assert.Equal("borrowed", r.Status);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(l, $"equipment-loans/{r.Id}/cancel", new EquipmentCommand(r.Version, null))).StatusCode);
    }
}

[Collection("SQL Server")]
public class EquipmentReturnTests(SqlFactory factory) : EquipmentScenario(factory), IClassFixture<SqlFactory>
{
    [Fact]
    public async Task PartialReturnsReleaseOnlyVerifiedGoodAndReplayAcceptanceIsAtomic()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        var r = await Borrow(m, l, a, 4);
        r = await Ok<EquipmentLoanView>(Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, 2, 1, 0, null)));
        Assert.Equal(1, (await FreshAsset(m, a.Id)).AvailableNow); // declaration has no effect
        var key = Key(); var input = new AcceptEquipmentReturn(r.Version, r.Returns.Single().Id, 1, 1, 0, "Thực nhận 2");
        r = await Ok<EquipmentLoanView>(Post(m, $"equipment-loans/{r.Id}/accept-return", input, key));
        var replay = await Ok<EquipmentLoanView>(Post(m, $"equipment-loans/{r.Id}/accept-return", input, key));
        Assert.Equal(r.Version, replay.Version); Assert.Equal(2, r.Returned); Assert.Equal("borrowed", r.Status);
        var after = await FreshAsset(m, a.Id); Assert.Equal(2, after.AvailableNow); Assert.Equal(1, after.Damaged); Assert.Equal(2, after.Outstanding);
        r = await Ok<EquipmentLoanView>(Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, 1, 0, 1, null)));
        var pending = r.Returns.Single(x => x.AcceptedAt == null);
        r = await Ok<EquipmentLoanView>(Post(m, $"equipment-loans/{r.Id}/accept-return", new AcceptEquipmentReturn(r.Version, pending.Id, 1, 0, 1, null)));
        Assert.Equal("completed", r.Status); Assert.Equal(4, r.Returned);
        after = await FreshAsset(m, a.Id); Assert.Equal(3, after.AvailableNow); Assert.Equal(1, after.Lost); Assert.Equal(0, after.Outstanding);
        Assert.Equal(2, r.Returns.Length);
        await using var scope = f.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var returnId = r.Returns[0].Id;
        var invalid = await Assert.ThrowsAsync<Microsoft.Data.SqlClient.SqlException>(() =>
            db.Database.ExecuteSqlInterpolatedAsync($"UPDATE GiangVien.PhieuTraThietBi SET AcceptedGood = NULL WHERE Id = {returnId}"));
        Assert.Equal(547, invalid.Number); // SQL CHECK must reject UNKNOWN/null, not just negative counts.
    }
    [Fact]
    public async Task ParallelAcceptanceCannotCreditTwiceAndExcessOrNegativeReturnsFail()
    {
        var (m, l, a) = await Setup(3); using var manager = m; using var lecturer = l;
        var r = await Borrow(m, l, a, 3);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, -1, 0, 0, null))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, 4, 0, 0, null))).StatusCode);
        r = await Ok<EquipmentLoanView>(Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(r.Version, 2, 0, 0, null)));
        var input = new AcceptEquipmentReturn(r.Version, r.Returns.Single().Id, 3, 0, 0, null);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(m, $"equipment-loans/{r.Id}/accept-return", input)).StatusCode);
        input = input with { Good = 2 };
        var results = await Task.WhenAll(Post(m, $"equipment-loans/{r.Id}/accept-return", input), Post(m, $"equipment-loans/{r.Id}/accept-return", input));
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.Conflict || x.StatusCode == HttpStatusCode.BadRequest);
        Assert.Equal(2, (await FreshAsset(m, a.Id)).AvailableNow);
    }
    [Fact]
    public async Task OverdueHandoverBlocksNewFutureBookingsUntilActuallyReturned()
    {
        var (m, l, a) = await Setup(2); using var manager = m; using var lecturer = l;
        var r = await Borrow(m, l, a, 2);
        // Prepare a historical loan without changing the wall clock or production records.
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var row = await db.EquipmentLoans.SingleAsync(x => x.Id == r.Id);
            row.Start = DateTimeOffset.UtcNow.AddDays(-2); row.End = DateTimeOffset.UtcNow.AddDays(-1); row.HandedAt = row.Start;
            await db.SaveChangesAsync();
        }
        var fresh = await l.GetFromJsonAsync<EquipmentLoanView>($"/api/v1/equipment-loans/{r.Id}"); Assert.True(fresh!.Overdue);
        var other = await Create(l, a, 1);
        Assert.Equal(HttpStatusCode.Conflict, (await Post(l, $"equipment-loans/{other.Id}/submit", new EquipmentCommand(other.Version, null))).StatusCode);
        fresh = await Ok<EquipmentLoanView>(Post(l, $"equipment-loans/{r.Id}/returns", new ReturnEquipment(fresh.Version, 2, 0, 0, null)));
        await Ok<EquipmentLoanView>(Post(m, $"equipment-loans/{r.Id}/accept-return", new AcceptEquipmentReturn(fresh.Version, fresh.Returns.Single().Id, 2, 0, 0, null)));
        await Action(l, other, "submit");
    }
    [Fact]
    public async Task ScopeAndPermissionsAreEnforcedOnDetailsListsAndStock()
    {
        var (m, l, a) = await Setup(); using var manager = m; using var lecturer = l;
        using var admin = await f.Login(); var outsider = await SqlFactory.Create(admin);
        using var other = await f.Login(outsider.UserName);
        var r = await Create(l, a, 1);
        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/api/v1/equipment-loans/{r.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await m.PostAsJsonAsync($"/api/v1/equipment/{a.Id}/stock", new ReceiveStock(1, a.Version, null))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Post(l, $"equipment/{a.Id}/stock", new ReceiveStock(1, a.Version, null))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await other.GetAsync("/api/v1/equipment-loans?review=true")).StatusCode);
        var u = await SqlFactory.Create(admin, "resources", "it");
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope", new ConfigureScope("unit", [], [], u.Version)));
        using var scoped = await f.Login(u.UserName);
        Assert.Equal(HttpStatusCode.Forbidden, (await Post(scoped, $"equipment/{a.Id}/stock", new ReceiveStock(1, a.Version, null))).StatusCode);
        await Action(l, r, "submit");
        var list = await scoped.GetFromJsonAsync<JsonElement>("/api/v1/equipment-loans?review=true&pageSize=100");
        Assert.Contains(list.GetProperty("items").EnumerateArray(), x => x.GetProperty("id").GetString() == r.Id); // borrower's unit matches
    }
}
