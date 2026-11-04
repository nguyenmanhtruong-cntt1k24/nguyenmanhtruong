using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

public class SqlFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string Password = "IntegrationOnly123!Test";
    public string DatabaseName { get; } = "HpuniTests_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N")[..8];
    private string? previousConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("Testing");

    public async Task InitializeAsync()
    {
        var raw = Environment.GetEnvironmentVariable("HPUNI_TEST_SQLSERVER")
            ?? "Server=.\\SQL2025;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        var cs = new SqlConnectionStringBuilder(raw) { InitialCatalog = DatabaseName };
        previousConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Hpuni");
        Environment.SetEnvironmentVariable("ConnectionStrings__Hpuni", cs.ConnectionString);
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        await db.Database.MigrateAsync();
        await Bootstrap.Run(db, scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(), Password);
        Console.WriteLine("SQL Server integration database retained: " + DatabaseName);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Hpuni", previousConnection);
        await DisposeAsync(); // Database intentionally retained; no automatic deletion.
    }

    public HttpClient Client() => CreateClient(new() { AllowAutoRedirect = false, HandleCookies = true });

    public static async Task Csrf(HttpClient c)
    {
        var data = await c.GetFromJsonAsync<JsonElement>("/api/v1/auth/csrf");
        c.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
        c.DefaultRequestHeaders.Add("X-CSRF-TOKEN", data.GetProperty("token").GetString());
    }

    public async Task<HttpClient> Login(string user = "admin.hpuni", string? password = null)
    {
        var c = Client();
        await Csrf(c);
        var response = await c.PostAsJsonAsync("/api/v1/auth/login", new { userName = user, password = password ?? Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await Csrf(c); // Antiforgery tokens are bound to the authenticated identity.
        return c;
    }

    public static async Task<UserView> Create(HttpClient admin, string role = "lecturer", string department = "it")
    {
        var name = "test." + Guid.NewGuid().ToString("N")[..12];
        var response = await admin.PostAsJsonAsync("/api/v1/users", new CreateUser(name, name + "@hpuni.test", "Tài khoản kiểm thử", department, Password, [role]));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<UserView>())!;
    }

    public static async Task<UserView> Fresh(HttpClient admin, string id)
    {
        var page = await admin.GetFromJsonAsync<JsonElement>("/api/v1/users?pageSize=100");
        return page.GetProperty("items").EnumerateArray().Select(x => x.Deserialize<UserView>(new JsonSerializerOptions(JsonSerializerDefaults.Web))!).Single(x => x.Id == id);
    }
}

[Collection("SQL Server")]
public class FoundationTests(SqlFactory f) : IClassFixture<SqlFactory>
{
    [Fact]
    public async Task PublicPortalServesOnlyExplicitAssets()
    {
        using var client = f.Client();
        foreach (var path in new[] { "/live.html", "/live.mjs", "/live.css", "/api-client.mjs", "/equipment-live.mjs", "/login.html", "/index.html", "/resources.html", "/admin.html", "/page-entry.mjs", "/server-session.mjs", "/server-resources.mjs" })
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/")).StatusCode);
        Assert.Equal("/login.html", (await client.GetAsync("/")).Headers.Location!.ToString());
        var login = await client.GetStringAsync("/login.html");
        Assert.Contains("method=\"post\"", login);
        Assert.Contains("type=\"submit\" disabled", login);
        foreach (var path in new[] { "/README.md", "/appsettings.json", "/api-client.test.mjs", "/login-demo.js" })
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path)).StatusCode);
    }

    [Fact]
    public async Task BootstrapRefusesToResetAnExistingDatabase()
    {
        await using var scope = f.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var before = await db.Users.CountAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => Bootstrap.Run(db,
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(), SqlFactory.Password));
        Assert.Equal(before, await db.Users.CountAsync());
    }

    [Fact]
    public async Task TwoAdministratorsCannotLockEachOtherConcurrently()
    {
        using var root = await f.Login();
        var a = await SqlFactory.Create(root, "admin", "administration");
        var b = await SqlFactory.Create(root, "admin", "administration");
        foreach (var user in new[] { a, b })
            Assert.Equal(HttpStatusCode.OK, (await root.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",
                new ConfigureScope("all", [], [], user.Version))).StatusCode);
        a = await SqlFactory.Fresh(root, a.Id); b = await SqlFactory.Fresh(root, b.Id);
        using var ca = await f.Login(a.UserName); using var cb = await f.Login(b.UserName);
        var results = await Task.WhenAll(
            ca.PostAsJsonAsync($"/api/v1/users/{b.Id}/status", new ChangeStatus(false, b.Version, null)),
            cb.PostAsJsonAsync($"/api/v1/users/{a.Id}/status", new ChangeStatus(false, a.Version, null)));
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Single(results, x => x.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized);
        a = await SqlFactory.Fresh(root, a.Id); b = await SqlFactory.Fresh(root, b.Id);
        Assert.NotEqual(a.Active, b.Active);
    }

    [Fact]
    public async Task MissingVersionCannotOverwriteAndScopedManagerCannotEscalatePrivileges()
    {
        using var root = await f.Login();
        var user = await SqlFactory.Create(root);
        var result = await root.PutAsJsonAsync($"/api/v1/users/{user.Id}", new UpdateUser(user.Email, "Không được ghi", "it", ["lecturer"], ""));
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        var configured = await root.PutAsJsonAsync($"/api/v1/users/{user.Id}/role-overrides",
            new ConfigureOverride("lecturer", false, "all", ["users.manage"], user.Version));
        Assert.Equal(HttpStatusCode.OK, configured.StatusCode);
        user = (await configured.Content.ReadFromJsonAsync<UserView>())!;
        Assert.Equal(HttpStatusCode.OK, (await root.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope", new ConfigureScope("all", [], [], user.Version))).StatusCode);
        using var delegated = await f.Login(user.UserName);
        // Viewing users is delegated, but assigning the administrator role is reserved to full system administrators.
        Assert.Equal(HttpStatusCode.OK, (await delegated.GetAsync("/api/v1/users")).StatusCode);
        var escalation = await delegated.PostAsJsonAsync("/api/v1/users", new CreateUser("escalation.test", "escalation@hpuni.test", "Không được tạo", "it", SqlFactory.Password, ["admin"]));
        Assert.Equal(HttpStatusCode.Forbidden, escalation.StatusCode);
    }

    [Fact]
    public async Task AuthenticationRequiresCsrfAndDoesNotRedirect()
    {
        using var c = f.Client();
        var noSession = await c.GetAsync("/api/v1/auth/me");
        Assert.Equal(HttpStatusCode.Unauthorized, noSession.StatusCode);
        Assert.Null(noSession.Headers.Location);
        var noCsrf = await c.PostAsJsonAsync("/api/v1/auth/login", new { userName = "admin.hpuni", password = SqlFactory.Password });
        Assert.Equal(HttpStatusCode.BadRequest, noCsrf.StatusCode);
        await SqlFactory.Csrf(c);
        var wrong = await c.PostAsJsonAsync("/api/v1/auth/login", new { userName = "admin.hpuni", password = "IncorrectPassword123" });
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
        using var admin = await f.Login();
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/v1/auth/me")).StatusCode);
        admin.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PostAsync("/api/v1/auth/logout", null)).StatusCode);
        await SqlFactory.Csrf(admin);
        Assert.Equal(HttpStatusCode.NoContent, (await admin.PostAsync("/api/v1/auth/logout", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await admin.GetAsync("/api/v1/auth/me")).StatusCode);
    }

    [Fact]
    public async Task LecturerCannotManageUsersOrAudit()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin);
        using var lecturer = await f.Login(user.UserName);
        Assert.Equal(HttpStatusCode.Forbidden, (await lecturer.GetAsync("/api/v1/users")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await lecturer.GetAsync("/api/v1/audit-events")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await lecturer.PostAsJsonAsync("/api/v1/departments",
            new SaveDepartment("HACK", "Ngoài quyền", null, true, null))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await lecturer.GetAsync("/api/v1/departments")).StatusCode);
    }

    [Fact]
    public async Task LockAndPasswordResetRevokeExistingSessionsImmediately()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin);
        using var lecturer = await f.Login(user.UserName);
        var locked = await admin.PostAsJsonAsync($"/api/v1/users/{user.Id}/status", new ChangeStatus(false, user.Version, null));
        Assert.Equal(HttpStatusCode.OK, locked.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await lecturer.GetAsync("/api/v1/auth/me")).StatusCode);
        user = await SqlFactory.Fresh(admin, user.Id);
        Assert.Equal(HttpStatusCode.OK, (await admin.PostAsJsonAsync($"/api/v1/users/{user.Id}/status", new ChangeStatus(true, user.Version, ""))).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await lecturer.GetAsync("/api/v1/auth/me")).StatusCode);
        using var newSession = await f.Login(user.UserName);
        user = await SqlFactory.Fresh(admin, user.Id);
        var reset = await admin.PostAsJsonAsync($"/api/v1/users/{user.Id}/reset-password", new { password = "ChangedOnly123!Test", version = user.Version });
        Assert.Equal(HttpStatusCode.NoContent, reset.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await newSession.GetAsync("/api/v1/auth/me")).StatusCode);
    }

    [Fact]
    public async Task SelfLockAndSelfPermissionRemovalRollbackWithoutAudit()
    {
        using var admin = await f.Login();
        var me = (await admin.GetFromJsonAsync<UserView>("/api/v1/auth/me"))!;
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PostAsJsonAsync($"/api/v1/users/{me.Id}/status", new ChangeStatus(false, me.Version, null))).StatusCode);
        var denial = await admin.PutAsJsonAsync($"/api/v1/users/{me.Id}/scope", new ConfigureScope("self", [], [], me.Version));
        Assert.Equal(HttpStatusCode.BadRequest, denial.StatusCode);
        var after = (await admin.GetFromJsonAsync<UserView>("/api/v1/auth/me"))!;
        Assert.Equal("all", after.Scope);
        Assert.Equal(me.Version, after.Version);
    }

    [Fact]
    public async Task ConcurrentEditsWithSameRowVersionOnlyCommitOnce()
    {
        using var admin = await f.Login();
        var code = "T" + Guid.NewGuid().ToString("N")[..10];
        var create = await admin.PostAsJsonAsync("/api/v1/departments", new SaveDepartment(code, "Kiểm thử " + code, null, true, null));
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var d = (await create.Content.ReadFromJsonAsync<DepartmentView>())!;
        var responses = await Task.WhenAll(
            admin.PutAsJsonAsync($"/api/v1/departments/{d.Id}", new SaveDepartment(code, d.Name + " A", "", true, d.Version)),
            admin.PutAsJsonAsync($"/api/v1/departments/{d.Id}", new SaveDepartment(code, d.Name + " B", "", true, d.Version)));
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task OverrideReplacesOnlyOneRoleAndFiltersUserLists()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin);
        var other = await SqlFactory.Create(admin, department: "economics");
        var update = await admin.PutAsJsonAsync($"/api/v1/users/{user.Id}",
            new UpdateUser(user.Email, user.DisplayName, user.DepartmentId, ["lecturer", "resources"], user.Version));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        user = (await update.Content.ReadFromJsonAsync<UserView>())!;
        var over = await admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/role-overrides",
            new ConfigureOverride("lecturer", false, "unit", ["users.manage"], user.Version));
        Assert.Equal(HttpStatusCode.OK, over.StatusCode);
        // Account self restriction still applies; an all-school role for other permissions cannot widen users.manage.
        using var scoped = await f.Login(user.UserName);
        var page = await scoped.GetFromJsonAsync<JsonElement>("/api/v1/users?pageSize=100");
        Assert.Equal(1, page.GetProperty("total").GetInt32());
        Assert.Equal(user.Id, page.GetProperty("items")[0].GetProperty("id").GetString());
        var me = (await scoped.GetFromJsonAsync<UserView>("/api/v1/auth/me"))!;
        Assert.Contains(me.Grants, g => g.RoleId == "resources" && g.Permissions.Contains("resources.manage"));
        Assert.DoesNotContain(me.Grants.Single(g => g.RoleId == "lecturer").Permissions, x => x == "rooms.book");
        Assert.DoesNotContain(page.GetProperty("items").EnumerateArray(), x => x.GetProperty("id").GetString() == other.Id);
        user = await SqlFactory.Fresh(admin, user.Id);
        Assert.Equal(HttpStatusCode.OK, (await admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/role-overrides",
            new ConfigureOverride("lecturer", true, "self", [], user.Version))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await scoped.GetAsync("/api/v1/users")).StatusCode);
    }

    [Fact]
    public async Task DuplicateEmailAndInactiveLinkedDepartmentAreRejected()
    {
        using var admin = await f.Login();
        var user = await SqlFactory.Create(admin);
        var duplicate = await admin.PostAsJsonAsync("/api/v1/users", new CreateUser("dup." + Guid.NewGuid().ToString("N")[..8], user.Email,
            "Trùng email", "it", SqlFactory.Password, ["lecturer"]));
        Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        var deps = await admin.GetFromJsonAsync<DepartmentView[]>("/api/v1/departments");
        var it = deps!.Single(x => x.Id == "it");
        Assert.Equal(HttpStatusCode.BadRequest, (await admin.PutAsJsonAsync("/api/v1/departments/it",
            new SaveDepartment(it.Code, it.Name, "", false, it.Version))).StatusCode);
        var audit = await admin.GetFromJsonAsync<JsonElement>("/api/v1/audit-events?pageSize=100");
        Assert.DoesNotContain(SqlFactory.Password, audit.ToString());
        Assert.DoesNotContain("passwordHash", audit.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
