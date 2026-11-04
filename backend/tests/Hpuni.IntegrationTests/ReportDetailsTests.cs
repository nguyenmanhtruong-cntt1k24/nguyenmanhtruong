using System.Net;
using System.Net.Http.Json;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class ReportDetailsTests(FinanceFactory factory) : EquipmentScenario(factory), IClassFixture<FinanceFactory>
{
    async Task<HttpClient> User(string role = "accounting", string department = "it", string[]? denied = null)
    {
        var admin = await factory.Admin(); var user = await SqlFactory.Create(admin, role, department);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope", new ConfigureScope("unit", [], denied ?? [], user.Version)));
        return await factory.Login(user.UserName);
    }
    async Task SeedFinance(int year, string department, params string[] vouchers)
    {
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        var actor = await db.Users.Where(x => x.UserName == "admin.hpuni").Select(x => x.Id).SingleAsync();
        foreach (var voucher in vouchers) db.FinanceEntries.Add(new FinanceEntry { DepartmentId = department, Year = year, Category = "general", Kind = "receipt", Amount = 123456, Voucher = voucher, NormalizedVoucher = voucher.ToUpperInvariant(), Date = new(year, 1, 2), ActorId = actor, Content = "<script>alert('test')</script> & dữ liệu" });
        await db.SaveChangesAsync();
    }
    [Fact]
    public async Task DetailsHaveStablePagesAndScopeAndRejectOutsideDepartment()
    {
        using var client = await User(); const int year = 2051;
        await SeedFinance(year, "it", "D1-" + Key(), "D2-" + Key(), "D3-" + Key()); await SeedFinance(year, "economics", "OUT-" + Key());
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); var entry = await db.FinanceEntries.FirstAsync(x => x.Year == year && x.DepartmentId == "it");
            db.FinanceReversals.Add(new() { EntryId = entry.Id, ActorId = entry.ActorId, Reason = "Đảo kiểm thử" }); await db.SaveChangesAsync();
        }
        var first = (await client.GetFromJsonAsync<ReportDetailPage>($"/api/v1/reports/details/finance?year={year}&page=1&pageSize=2"))!;
        var second = (await client.GetFromJsonAsync<ReportDetailPage>($"/api/v1/reports/details/finance?year={year}&page=2&pageSize=2"))!;
        Assert.Equal(3, first.Total); Assert.Equal(2, first.Items.Length); Assert.Single(second.Items); Assert.Empty(first.Items.Select(x => x.Id).Intersect(second.Items.Select(x => x.Id)));
        var again = (await client.GetFromJsonAsync<ReportDetailPage>($"/api/v1/reports/details/finance?year={year}&page=1&pageSize=2"))!; Assert.Equal(first.Items.Select(x => x.Id), again.Items.Select(x => x.Id));
        Assert.All(first.Items.Concat(second.Items), row => Assert.Contains("Khoa CNTT", row.Cells));
        Assert.Single(first.Items.Concat(second.Items), row => row.Cells.Contains("Đã đảo"));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/api/v1/reports/details/finance?year={year}&departmentId=economics")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/api/v1/reports/details/finance/export/xlsx?year={year}&departmentId=economics")).StatusCode);
    }
    [Fact]
    public async Task ExportsReadPermissionsRemainIndependentAndFinanceNeedsBudgetRead()
    {
        using var exportOnly = await User(denied: ["reports.read"]); using var readOnly = await User(denied: ["reports.export"]); using var manager = await User("resources");
        Assert.Equal(HttpStatusCode.Forbidden, (await exportOnly.GetAsync("/api/v1/reports/details/assets?year=2052")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await exportOnly.GetAsync("/api/v1/reports/details/assets/export/xlsx?year=2052")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await readOnly.GetAsync("/api/v1/reports/details/assets?year=2052")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await readOnly.GetAsync("/api/v1/reports/details/assets/export/print?year=2052")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await manager.GetAsync("/api/v1/reports/details/finance?year=2052")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await manager.GetAsync("/api/v1/reports/details/finance/export/csv?year=2052")).StatusCode);
    }
    [Fact]
    public async Task ExcelIsRealWorkbookAndUntrustedTextIsNotFormulaAndPrintIsEscaped()
    {
        using var client = await User(); const int year = 2053; var voucher = "=SUM(1,1)";
        await SeedFinance(year, "it", voucher);
        var response = await client.GetAsync($"/api/v1/reports/details/finance/export/xlsx?year={year}"); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Content.Headers.ContentType!.MediaType);
        var bytes = await response.Content.ReadAsByteArrayAsync(); using var stream = new MemoryStream(bytes); using var workbook = new XLWorkbook(stream); var sheet = workbook.Worksheet(1);
        Assert.Equal(voucher, sheet.Cell(6, 2).GetString()); Assert.False(sheet.Cell(6, 2).HasFormula); Assert.Equal(123456d, sheet.Cell(6, 6).GetDouble());
        using (var validationStream = new MemoryStream(bytes)) using (var document = SpreadsheetDocument.Open(validationStream, false)) Assert.Empty(new OpenXmlValidator().Validate(document));
        var csv = await client.GetStringAsync($"/api/v1/reports/details/finance/export/csv?year={year}"); Assert.Contains("\"'=SUM(1,1)\"", csv);
        var print = await client.GetAsync($"/api/v1/reports/details/finance/export/print?year={year}"); var html = await print.Content.ReadAsStringAsync();
        Assert.Contains("&lt;script&gt;", html); Assert.DoesNotContain("<script>", html); Assert.Contains("frame-ancestors 'none'", print.Headers.GetValues("Content-Security-Policy").Single());
        await using var scope = f.Services.CreateAsyncScope(); var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(3, await db.AuditEvents.CountAsync(x => x.Action == "reports.export.details" && x.TargetId == "finance"));
    }
    [Fact]
    public async Task LargeExportFailsExplicitlyWhilePageStillWorks()
    {
        using var client = await User(); const int year = 2056;
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); var actor = await db.Users.Where(x => x.UserName == "admin.hpuni").Select(x => x.Id).SingleAsync();
            db.FinanceEntries.AddRange(Enumerable.Range(0, ReportDetailsService.ExportLimit + 1).Select(i => new FinanceEntry { DepartmentId = "it", Year = year, Category = "general", Kind = "receipt", Amount = 1, Voucher = "LIMIT-" + i, NormalizedVoucher = "LIMIT-" + i, Date = new(year, 1, 2), ActorId = actor, Content = "Kiểm tra giới hạn xuất" })); await db.SaveChangesAsync();
        }
        var page = (await client.GetFromJsonAsync<ReportDetailPage>($"/api/v1/reports/details/finance?year={year}&pageSize=20"))!; Assert.Equal(10001, page.Total); Assert.Equal(20, page.Items.Length);
        var export = await client.GetAsync($"/api/v1/reports/details/finance/export/xlsx?year={year}"); Assert.Equal(HttpStatusCode.BadRequest, export.StatusCode); Assert.Contains("10.000", await export.Content.ReadAsStringAsync());
    }
    [Fact]
    public async Task ResourceDetailsUseResourceOwnerAndVietnamYearBoundary()
    {
        using var manager = await User("resources", "economics");
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); var actor = await db.Users.Where(x => x.UserName == "admin.hpuni").Select(x => x.Id).SingleAsync();
            var asset = new AssetGroup { Code = "DT-" + Key()[..8], Name = "Thiết bị chủ sở hữu", DepartmentId = "economics", Unit = "chiếc", Category = "Thiết bị", Total = 4, Damaged = 1, Lost = 1 }; db.AssetGroups.Add(asset);
            db.EquipmentLoans.Add(new() { AssetGroupId = asset.Id, DepartmentId = "it", OwnerId = actor, Quantity = 1, Status = "pending", Start = new(2054, 1, 1, 0, 30, 0, TimeSpan.FromHours(7)), End = new(2054, 1, 1, 1, 30, 0, TimeSpan.FromHours(7)) });
            var room = new Room { Code = "DT-" + Key()[..8], Name = "Phòng liên đơn vị", DepartmentId = "economics", Capacity = 20, Type = "Phòng học" }; db.Rooms.Add(room);
            db.RoomRequests.Add(new() { RoomId = room.Id, DepartmentId = "it", OwnerId = actor, Attendees = 5, Start = new(2054, 1, 1, 0, 30, 0, TimeSpan.FromHours(7)), End = new(2054, 1, 1, 1, 30, 0, TimeSpan.FromHours(7)), Status = "pending" }); await db.SaveChangesAsync();
        }
        var loans = (await manager.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/loans?year=2054"))!; Assert.Single(loans.Items); Assert.Contains("Khoa Kinh tế", loans.Items[0].Cells); Assert.Contains("2054-01-01 00:30", loans.Items[0].Cells);
        var previous = (await manager.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/loans?year=2053"))!; Assert.Empty(previous.Items);
        var rooms = (await manager.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/rooms?year=2054"))!; Assert.Single(rooms.Items);
        var assets = (await manager.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/assets?year=2054"))!; var row = Assert.Single(assets.Items); Assert.Equal("2", row.Cells[5]); Assert.Contains("không phải tồn cuối năm", assets.Note);
    }
    [Theory]
    [InlineData("finance?year=1999")]
    [InlineData("finance?year=2055&page=0")]
    [InlineData("finance?year=2055&pageSize=101")]
    [InlineData("assets?year=2055&category=general")]
    public async Task InvalidFiltersAreRejected(string query)
    {
        using var client = await User(); Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/reports/details/" + query)).StatusCode);
    }
    [Fact]
    public async Task HistoricalAcquisitionTotalsRequireCompleteCoverageAndNeverBecomeCurrentValue()
    {
        using var manager = await User("resources"); string missingId; string actor;
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); actor = await db.Users.Where(x => x.UserName == "admin.hpuni").Select(x => x.Id).SingleAsync();
            var first = new AssetGroup { Code = "COV-A-" + Key()[..8], Name = "Đã kê khai", DepartmentId = "it", Unit = "chiếc", Category = "Thiết bị", Total = 2 };
            var missing = new AssetGroup { Code = "COV-B-" + Key()[..8], Name = "Chưa kê khai", DepartmentId = "it", Unit = "chiếc", Category = "Thiết bị", Total = 0, Active = false }; missingId = missing.Id;
            var outside = new AssetGroup { Code = "COV-X-" + Key()[..8], Name = "Ngoài phạm vi", DepartmentId = "economics", Unit = "chiếc", Category = "Thiết bị", Total = 1 }; db.AssetGroups.AddRange(first, missing, outside);
            db.FixedAssetProfiles.AddRange(new FixedAssetProfile { AssetGroupId = first.Id, AcquisitionValue = 100, AcquisitionDate = new(2020, 1, 1), Source = "Nguồn", Reference = "COV-A", ActorId = actor }, new FixedAssetProfile { AssetGroupId = outside.Id, AcquisitionValue = 999999999, AcquisitionDate = new(2020, 1, 1), Source = "Nguồn", Reference = "COV-X", ActorId = actor }); await db.SaveChangesAsync();
        }
        var incomplete = (await manager.GetFromJsonAsync<ResourceReport>("/api/v1/reports/resources?year=2070&departmentId=it"))!; var row = Assert.Single(incomplete.Rows);
        Assert.Equal(2, row.AssetGroups); Assert.Equal(1, row.AcquisitionProfiles); Assert.Equal(1, row.MissingAcquisitionProfiles); Assert.False(row.HistoricalDeclaredValueComplete); Assert.Null(row.HistoricalDeclaredValue); Assert.Null(row.PurchaseValue);
        var details = (await manager.GetFromJsonAsync<ReportDetailPage>("/api/v1/reports/details/assets?year=2070&departmentId=it"))!; Assert.Equal("", Assert.Single(details.Items, x => x.Id == missingId).Cells[9]);
        var excel = await manager.GetByteArrayAsync("/api/v1/reports/details/assets/export/xlsx?year=2070&departmentId=it"); using (var stream = new MemoryStream(excel)) using (var workbook = new XLWorkbook(stream)) Assert.True(workbook.Worksheet(1).Cell(7, 10).IsEmpty());
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>(); db.FixedAssetProfiles.Add(new() { AssetGroupId = missingId, AcquisitionValue = 200, AcquisitionDate = new(2020, 1, 2), Source = "Nguồn", Reference = "COV-B", ActorId = actor }); await db.SaveChangesAsync();
        }
        var complete = (await manager.GetFromJsonAsync<ResourceReport>("/api/v1/reports/resources?year=2070"))!; row = Assert.Single(complete.Rows); Assert.Equal("it", row.DepartmentId); Assert.Equal(2, row.AcquisitionProfiles); Assert.True(row.HistoricalDeclaredValueComplete); Assert.Equal(300, row.HistoricalDeclaredValue); Assert.Null(row.PurchaseValue);
        var csv = await manager.GetStringAsync("/api/v1/reports/resources.csv?year=2070"); Assert.Contains("Nguyên giá lịch sử đủ kê khai", csv); Assert.Contains("\"300\"", csv); Assert.DoesNotContain("999999999", csv);
    }
}
