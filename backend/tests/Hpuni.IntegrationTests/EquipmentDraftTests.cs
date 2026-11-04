using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Data.Common;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

sealed class ReturnsQueryCounter : DbCommandInterceptor
{
    public int Count;
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken ct = default)
    {
        if (command.CommandText.Contains("PhieuTraThietBi")) Interlocked.Increment(ref Count);
        return base.ReaderExecutingAsync(command, eventData, result, ct);
    }
}

[Collection("SQL Server")]
public class EquipmentDraftTests(SqlFactory factory) : EquipmentScenario(factory), IClassFixture<SqlFactory>
{
    static Task<HttpResponseMessage> Put(HttpClient c, string id, SaveEquipmentLoan body) =>
        c.PutAsJsonAsync($"/api/v1/equipment-loans/{id}", body);
    [Fact]
    public async Task OwnerUpdatesDraftKeepingAssetAndAdvancingVersion()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        var d = await Create(l, a, 1);
        var u = await Ok<EquipmentLoanView>(Put(l, d.Id, new SaveEquipmentLoan(a.Id, At(9), At(11), 2, "Đổi mục đích", d.Version)));
        Assert.Equal(a.Id, u.AssetGroupId); Assert.Equal(2, u.Quantity); Assert.Equal("Đổi mục đích", u.Purpose);
        Assert.Equal("draft", u.Status); Assert.NotEqual(d.Version, u.Version);
        var got = await l.GetFromJsonAsync<EquipmentLoanView>($"/api/v1/equipment-loans/{d.Id}");
        Assert.Equal(2, got!.Quantity); Assert.Equal(At(9).ToUniversalTime(), got.Start.ToUniversalTime());
        await Action(l, u, "submit"); // phiếu sau khi sửa vẫn gửi được
    }
    [Fact]
    public async Task WrongOwnerOrPermissionCannotUpdateAndInvalidInputIsRejected()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        using var admin = await f.Login(); var o = await SqlFactory.Create(admin); using var other = await f.Login(o.UserName);
        var d = await Create(l, a, 1);
        var body = new SaveEquipmentLoan(a.Id, At(9), At(11), 2, null, d.Version);
        Assert.Contains((await Put(other, d.Id, body)).StatusCode, new[] { HttpStatusCode.NotFound, HttpStatusCode.Forbidden });
        Assert.Contains((await Put(m, d.Id, body)).StatusCode, new[] { HttpStatusCode.NotFound, HttpStatusCode.Forbidden });
        Assert.Equal(HttpStatusCode.BadRequest, (await Put(l, d.Id, body with { Quantity = 99 })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Put(l, d.Id, body with { Start = At(11), End = At(9) })).StatusCode);
        var b = await Ok<AssetGroupView>(Post(m, "equipment", new SaveAssetGroup("TB" + Key()[..10], "Loa", "Giảng dạy", "chiếc", "administration", true, null)));
        Assert.Equal(HttpStatusCode.BadRequest, (await Put(l, d.Id, body with { AssetGroupId = b.Id })).StatusCode);
        var same = await l.GetFromJsonAsync<EquipmentLoanView>($"/api/v1/equipment-loans/{d.Id}");
        Assert.Equal(1, same!.Quantity); Assert.Equal(d.Version, same.Version); // không bị sửa
    }
    [Fact]
    public async Task StaleVersionAndNonDraftAreRejected()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        var d = await Create(l, a, 1);
        var u = await Ok<EquipmentLoanView>(Put(l, d.Id, new SaveEquipmentLoan(a.Id, At(8), At(10), 2, null, d.Version)));
        Assert.Equal(HttpStatusCode.Conflict, (await Put(l, d.Id, new SaveEquipmentLoan(a.Id, At(8), At(10), 3, null, d.Version))).StatusCode);
        var s = await Action(l, u, "submit");
        Assert.Equal(HttpStatusCode.BadRequest, (await Put(l, d.Id, new SaveEquipmentLoan(a.Id, At(8), At(10), 3, null, s.Version))).StatusCode);
        Assert.Equal(2, (await l.GetFromJsonAsync<EquipmentLoanView>($"/api/v1/equipment-loans/{d.Id}"))!.Quantity);
    }
    [Fact]
    public async Task BatchReturnsKeepContentAndUseOneQueryForThePage()
    {
        var (m, l, a) = await Setup(5); using var manager = m; using var lecturer = l;
        var r1 = await Borrow(m, l, a, 1); var r2 = await Borrow(m, l, a, 1); var r3 = await Borrow(m, l, a, 1);
        r1 = await Ok<EquipmentLoanView>(Post(l, $"equipment-loans/{r1.Id}/returns", new ReturnEquipment(r1.Version, 1, 0, 0, "trả 1")));
        var list = await l.GetFromJsonAsync<JsonElement>("/api/v1/equipment-loans?pageSize=100");
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var items = list.GetProperty("items").EnumerateArray().Select(x => x.Deserialize<EquipmentLoanView>(web)!).ToDictionary(x => x.Id);
        Assert.Single(items[r1.Id].Returns); Assert.Equal("trả 1", items[r1.Id].Returns[0].Note);
        Assert.Empty(items[r2.Id].Returns); Assert.Empty(items[r3.Id].Returns);
        // Đếm số câu lệnh SQL chạm EquipmentReturns khi dựng view cho cả trang.
        await using var scope = f.Services.CreateAsyncScope();
        var counter = new ReturnsQueryCounter();
        var options = new DbContextOptionsBuilder<HpuniDbContext>(scope.ServiceProvider.GetRequiredService<DbContextOptions<HpuniDbContext>>())
            .AddInterceptors(counter).Options;
        await using var db = new HpuniDbContext(options);
        var rows = await db.EquipmentLoans.AsNoTracking().Include(x => x.Asset).Where(x => x.AssetGroupId == a.Id).ToArrayAsync();
        counter.Count = 0;
        var views = await new EquipmentService(db, null!, null!, null!).Views(rows, default);
        Assert.Equal(3, views.Length); Assert.Equal(1, counter.Count); // trước đây: 3
    }

    [Fact]
    public async Task OwnerLosesDraftAccessWhenAccountScopeNoLongerIncludesItsDepartment()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin);
        var (m, l, a) = await Setup(); using var manager = m; using var setupLecturer = l;
        using var owner = await f.Login(user.UserName);
        var draft = await Create(owner, a, 1);
        var fresh = await SqlFactory.Fresh(admin, user.Id);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",
            new ConfigureScope("units", ["economics"], [], fresh.Version)));
        var response = await Put(owner, draft.Id, new SaveEquipmentLoan(a.Id, At(9), At(11), 2, null, draft.Version));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await using var scope = f.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var unchanged = await db.EquipmentLoans.AsNoTracking().SingleAsync(x => x.Id == draft.Id);
        Assert.Equal(1, unchanged.Quantity);
        Assert.Equal(draft.Version, Convert.ToBase64String(unchanged.RowVersion));
    }
}
