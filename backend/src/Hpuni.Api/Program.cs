using System.Threading.RateLimiting;
using Hpuni.Api;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var trustedProxy = TrustedProxyConfiguration.Read(builder.Configuration);
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 1024 * 1024);
var connection = builder.Configuration.GetConnectionString("Hpuni")
    ?? throw new InvalidOperationException("Cần cấu hình ConnectionStrings:Hpuni.");
builder.Services.AddDbContext<HpuniDbContext>(o => o.UseSqlServer(connection,
    sql => sql.MigrationsHistoryTable(VietnameseTableNames.MigrationHistory, ActorTableSchemas.HistorySchema)));
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 12;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireDigit = true;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<HpuniDbContext>()
    .AddUserStore<FrontendUserStore>().AddRoleStore<FrontendRoleStore>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = "hpuni.auth";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing")
        ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.SlidingExpiration = false;
    o.EventsType = typeof(SessionCookieEvents);
});
builder.Services.AddScoped<SessionCookieEvents>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<RoomLogService>();
builder.Services.AddScoped<IdempotentCommands>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<AssetOperationsService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<FinanceControlService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<ReportDetailsService>();
builder.Services.AddScoped<AssetLifecycleService>();
builder.Services.AddOptions<AttachmentScanOptions>().Bind(builder.Configuration.GetSection("AttachmentScan"));
builder.Services.AddSingleton<IAttachmentScanner,ClamAvAttachmentScanner>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<VoucherAttachmentService>();
builder.Services.AddOptions<RoomOptions>().Bind(builder.Configuration.GetSection("Rooms"))
    .Validate(o => o.OpenHour >= 0 && o.CloseHour <= 24 && o.OpenHour < o.CloseHour, "Giờ phục vụ không hợp lệ.").ValidateOnStart();
builder.Services.AddAntiforgery(o =>
{
    o.HeaderName = "X-CSRF-TOKEN";
    o.Cookie.Name = "hpuni.csrf";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing")
        ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
});
builder.Services.AddControllers(o => o.Filters.Add<ApiAntiforgeryFilter>());
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
var keys = builder.Configuration["DataProtection:Path"];
var protection = builder.Services.AddDataProtection().SetApplicationName("HPUNI-STII");
if (!string.IsNullOrWhiteSpace(keys)) protection.PersistKeysToFileSystem(new DirectoryInfo(keys));
if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing") && string.IsNullOrWhiteSpace(keys))
    throw new InvalidOperationException("Cần cấu hình DataProtection:Path bền vững trước vận hành.");

var app = builder.Build();
// Database lifecycle commands are explicit; normal startup never resets or migrates existing data.
if (args.Contains("--migrate") || args.Contains("--bootstrap") || args.Contains("--seed-samples") || args.Contains("--ensure-khtc"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
    if (args.Contains("--ensure-khtc"))
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        await TransactionLocks.Acquire(db, "identity", default);
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var accounting = await db.Departments.SingleOrDefaultAsync(d => d.Id == "accounting");
        if (accounting != null) { accounting.Name = "Kế toán"; accounting.NormalizedName = accounting.Name.ToUpperInvariant(); }
        if (!await db.Departments.AnyAsync(d => d.Id == "khtc"))
            db.Departments.Add(new() { Id = "khtc", Code = "KHTC", Name = "KHTC", NormalizedName = "KHTC" });
        await db.SaveChangesAsync();
        if (!await roles.RoleExistsAsync("Cán bộ thẩm định KHTC"))
        {
            if (await db.Roles.AnyAsync(x => x.Id == "khtc")) throw new InvalidOperationException("ID khtc đã tồn tại với tên khác.");
            AdminService.Check(await roles.CreateAsync(new() { Id = "khtc", Name = "Cán bộ thẩm định KHTC", Scope = Hpuni.Domain.Scopes.All, BuiltIn = true }));
            db.RolePermissions.AddRange(new[] { "budget.read", "budget.appraise", "reports.read" }.Select(p => new Hpuni.Domain.RolePermission { RoleId = "khtc", Permission = p }));
            await db.SaveChangesAsync();
        }
        await tx.CommitAsync();
        Console.WriteLine("Vai trò KHTC sẵn sàng; không tự gán quyền cho tài khoản hiện có.");
    }
    if (args.Contains("--migrate"))
    {
        // Upgrade an existing English-named history before EF reads applied migrations.
        // New databases are created by MigrateAsync below.
        if (await db.Database.CanConnectAsync())
            await db.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NOT NULL
                BEGIN
                    IF OBJECT_ID(N'dbo.LichSuCapNhatCauTruc', N'U') IS NOT NULL
                        THROW 51000, 'Both migration history tables exist; manual review required.', 1;
                    EXEC sys.sp_rename N'dbo.__EFMigrationsHistory', N'LichSuCapNhatCauTruc', N'OBJECT';
                END
                IF OBJECT_ID(N'dbo.LichSuCapNhatCauTruc', N'U') IS NOT NULL
                BEGIN
                    IF OBJECT_ID(N'QuanTriVien.LichSuCapNhatCauTruc', N'U') IS NOT NULL
                        THROW 51001, 'Both history schemas exist; manual review required.', 1;
                    IF SCHEMA_ID(N'QuanTriVien') IS NULL EXEC(N'CREATE SCHEMA QuanTriVien AUTHORIZATION dbo');
                    ALTER SCHEMA QuanTriVien TRANSFER dbo.LichSuCapNhatCauTruc;
                END
                IF OBJECT_ID(N'QuanTriVien.LichSuCapNhatCauTruc', N'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH(N'QuanTriVien.LichSuCapNhatCauTruc', N'MigrationId') IS NOT NULL
                    BEGIN
                        IF COL_LENGTH(N'QuanTriVien.LichSuCapNhatCauTruc', N'MaCapNhat') IS NOT NULL
                            THROW 51002, 'Both history ID columns exist; manual review required.', 1;
                        EXEC sys.sp_rename N'QuanTriVien.LichSuCapNhatCauTruc.MigrationId', N'MaCapNhat', N'COLUMN';
                    END
                    IF COL_LENGTH(N'QuanTriVien.LichSuCapNhatCauTruc', N'ProductVersion') IS NOT NULL
                    BEGIN
                        IF COL_LENGTH(N'QuanTriVien.LichSuCapNhatCauTruc', N'PhienBanSanPham') IS NOT NULL
                            THROW 51003, 'Both history version columns exist; manual review required.', 1;
                        EXEC sys.sp_rename N'QuanTriVien.LichSuCapNhatCauTruc.ProductVersion', N'PhienBanSanPham', N'COLUMN';
                    END
                END
                """);
        await db.Database.MigrateAsync();
    }
    if (args.Contains("--bootstrap"))
    {
        var password = builder.Configuration["Bootstrap:Password"];
        if (password == null && builder.Environment.IsDevelopment() && !Console.IsInputRedirected)
        {
            Console.Write("Mật khẩu mới cho admin.hpuni (nhập ẩn, 12–128 ký tự): ");
            var input = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace) { if (input.Length > 0) input.Length--; }
                else if (!char.IsControl(key.KeyChar) && input.Length <= 128) input.Append(key.KeyChar);
            }
            Console.WriteLine();
            password = input.ToString();
            input.Clear();
        }
        await Bootstrap.Run(db,
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(),
            password ?? throw new InvalidOperationException("Cần Bootstrap:Password; không có mật khẩu mặc định."));
    }
    if (args.Contains("--seed-samples"))
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(await SampleData.Run(db)));
    return;
}
if (trustedProxy != null) app.UseForwardedHeaders(trustedProxy);
app.Use(async (context, next) =>
{
    // OnStarting runs after exception handling, including Response.Clear().
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["Referrer-Policy"] = "same-origin";
        if (context.Request.Path.StartsWithSegments("/api")) context.Response.Headers.CacheControl = "no-store";
        return Task.CompletedTask;
    });
    try { await next(); }
    catch (Exception ex) when (!context.Response.HasStarted && ex is not OperationCanceledException)
    {
        var (status, code, message) = ex switch
        {
            AppException a => (a.Status, a.Code, a.Message),
            DbUpdateConcurrencyException => (409, "STALE_VERSION", "Dữ liệu đã thay đổi. Vui lòng tải lại."),
            DbUpdateException d when d.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 }
                => (409, "DUPLICATE_DATA", "Mã, tên đăng nhập, email hoặc dữ liệu đã tồn tại."),
            _ => (500, "SERVER_ERROR", "Không thể xử lý yêu cầu. Vui lòng thử lại.")
        };
        if (status == 500) app.Logger.LogError(ex, "Request failed {RequestId}", context.TraceIdentifier);
        context.Response.Clear();
        await Results.Problem(statusCode: status, title: message, extensions: new Dictionary<string, object?>
        { ["code"] = code, ["request_id"] = context.TraceIdentifier }).ExecuteAsync(context);
    }
});
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseAuthentication();
// Only explicitly selected public assets are served; workspace documents never become public.
var frontend = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "..", "frontend"));
if (!Directory.Exists(frontend)) frontend = Path.Combine(AppContext.BaseDirectory, "PublicPortal");
if (Directory.Exists(frontend))
{
    var publicFiles = FrontendAssets.Paths;
    app.UseWhen(c => publicFiles.Contains(c.Request.Path.Value ?? ""), branch =>
        branch.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(frontend),
            OnPrepareResponse = c => c.Context.Response.Headers.CacheControl = "no-cache" }));
    app.MapGet("/", () => Results.Redirect("/login.html"));
}
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health/live", () => Results.Ok(new { status = "ok" }));
app.MapGet("/health/ready", async (HpuniDbContext db, CancellationToken ct) =>
{
    try
    {
        return await db.Database.CanConnectAsync(ct) && !(await db.Database.GetPendingMigrationsAsync(ct)).Any()
            ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503);
    }
    catch (Microsoft.Data.SqlClient.SqlException) { return Results.StatusCode(503); }
});
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.Run();

public partial class Program;
