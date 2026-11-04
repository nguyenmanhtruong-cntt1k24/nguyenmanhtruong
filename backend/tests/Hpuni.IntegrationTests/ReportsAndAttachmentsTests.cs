using System.Net;
using System.Net.Http.Json;
using System.Text;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class ReportsAndAttachmentsTests(FinanceFactory factory):EquipmentScenario(factory),IClassFixture<FinanceFactory>
{
    async Task<HttpClient> User(string role="accounting",string department="it",string scope="all",string[]? denied=null)
    {
        var admin=await factory.Admin();var u=await SqlFactory.Create(admin,role,department);
        await Ok<UserView>(admin.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope(scope,[],denied??[],u.Version)));
        return await factory.Login(u.UserName);
    }
    static readonly byte[] Pdf=Encoding.ASCII.GetBytes("%PDF-1.4\n% test attachment\n%%EOF\n");
    static Task<HttpResponseMessage> Upload(HttpClient c,string entry,string name,byte[]? bytes=null,string? key=null)
    {
        var form=new MultipartFormDataContent();form.Add(new ByteArrayContent(bytes??Pdf),"file",name);
        var r=new HttpRequestMessage(HttpMethod.Post,$"/api/v1/finance/entries/{entry}/attachments"){Content=form};r.Headers.Add("Idempotency-Key",key??Key());return c.SendAsync(r);
    }
    static Task<FinanceEntryView> Receipt(HttpClient c,int year=2045,string department="it")=>Ok<FinanceEntryView>(Post(c,"finance/entries/receipt",new RecordFinanceEntry(department,year,"general",1,Key(),new DateOnly(year,1,2),"Chứng từ tệp")));
    [Fact]
    public async Task FinanceReportReconcilesBudgetAndSeparatesPendingAndHistoricalCosts()
    {
        using var c=await User();const int year=2040;
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var actor=await db.Users.Where(u=>u.UserName=="admin.hpuni").Select(u=>u.Id).SingleAsync();
            var asset=new AssetGroup{Code="R-"+Key()[..12],Name="Tài sản báo cáo",DepartmentId="it",Category="Thiết bị",Unit="chiếc",Total=1};db.AssetGroups.Add(asset);
            var jobs=new[]{100m,200m,50m}.Select(cost=>new MaintenanceOrder{AssetGroupId=asset.Id,DepartmentId="it",Quantity=1,Start=DateTimeOffset.UtcNow.AddDays(-2),End=DateTimeOffset.UtcNow.AddDays(-1),Status="completed",Result="good",Cost=cost}).ToArray();db.MaintenanceOrders.AddRange(jobs);
            var proposal=new BudgetProposal{DepartmentId="it",Year=year,Category="maintenance",Title="Dự toán",OwnerId=actor,Status="approved",Amount=1000};db.BudgetProposals.Add(proposal);
            db.BudgetAllocations.Add(new(){ProposalId=proposal.Id,DepartmentId="it",Year=year,Category="maintenance",Amount=1000});
            var paid=new PaymentRequest{DepartmentId="it",Year=year,Amount=100,MaintenanceId=jobs[0].Id,Status="paid"};db.PaymentRequests.Add(paid);
            db.PaymentRequests.AddRange(new PaymentRequest{DepartmentId="it",Year=year,Amount=200,MaintenanceId=jobs[1].Id},new PaymentRequest{DepartmentId="it",Year=year,Amount=300});
            db.FinanceEntries.AddRange(new FinanceEntry{DepartmentId="it",Year=year,Category="maintenance",Kind="expense",Amount=100,Voucher=Key(),NormalizedVoucher=Key(),Date=new(year,1,2),ActorId=actor,PaymentRequestId=paid.Id},new FinanceEntry{DepartmentId="it",Year=year,Category="maintenance",Kind="receipt",Amount=50,Voucher=Key(),NormalizedVoucher=Key(),Date=new(year,1,3),ActorId=actor});
            await db.SaveChangesAsync();
        }
        var report=await c.GetFromJsonAsync<FinanceReport>($"/api/v1/reports/finance?year={year}&departmentId=it&category=maintenance");
        var row=Assert.Single(report!.Rows);Assert.Equal(1000,row.Allocated);Assert.Equal(100,row.Spent);Assert.Equal(50,row.Received);Assert.Equal(100,row.MaintenanceSpent);Assert.Equal(900,row.Available);
        var pending=Assert.Single(report.PendingCosts);Assert.Equal(500,pending.Pending);Assert.Equal(200,pending.MaintenancePending);Assert.Equal(50,pending.UnqueuedMaintenanceAllTime);
        var balance=await c.GetFromJsonAsync<BudgetBalance>($"/api/v1/finance/balance?departmentId=it&year={year}&category=maintenance");Assert.Equal(balance!.Available,row.Available);
        var csv=await c.GetStringAsync($"/api/v1/reports/finance.csv?year={year}&departmentId=it&category=maintenance");Assert.Contains("\"1000\",\"50\",\"100\",\"900\",\"100\"",csv);Assert.Contains("\"500\",\"200\",\"50\"",csv);
    }
    [Fact]
    public async Task ResourceReportAttributesUsageToResourceOwnerAndKeepsUnknownValueNull()
    {
        using var c=await User("resources","economics","unit");
        await using(var scope=f.Services.CreateAsyncScope()){
            var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var actor=await db.Users.Where(u=>u.UserName=="admin.hpuni").Select(u=>u.Id).SingleAsync();
            var a=new AssetGroup{Code="S-"+Key()[..12],Name="Tài sản liên đơn vị",DepartmentId="economics",Category="Thiết bị",Unit="chiếc",Total=5,Damaged=1,Lost=1};db.AssetGroups.Add(a);
            db.EquipmentLoans.Add(new(){AssetGroupId=a.Id,DepartmentId="it",OwnerId=actor,Quantity=2,Returned=1,Status="borrowed",Start=new(2041,12,31,23,30,0,TimeSpan.FromHours(7)),End=new(2042,1,1,0,30,0,TimeSpan.FromHours(7)),HandedAt=DateTimeOffset.UtcNow.AddDays(-3)});
            var room=new Room{Code="P-"+Key()[..12],Name="Phòng báo cáo",Type="Phòng học",DepartmentId="economics",Capacity=20};db.Rooms.Add(room);
            db.RoomRequests.Add(new(){RoomId=room.Id,DepartmentId="it",OwnerId=actor,Attendees=10,Start=new(2042,1,1,0,30,0,TimeSpan.FromHours(7)),End=new(2042,1,1,1,30,0,TimeSpan.FromHours(7)),Status="approved"});await db.SaveChangesAsync();
        }
        var r=await c.GetFromJsonAsync<ResourceReport>("/api/v1/reports/resources?year=2042&departmentId=economics");var row=Assert.Single(r!.Rows);
        Assert.Equal(5,row.Owned);Assert.Equal(3,row.Good);Assert.Equal(1,row.Outstanding);Assert.Equal(0,row.LoansStartingInYear);Assert.Equal(1,row.ApprovedRoomRequestsStartingInYear);Assert.Null(row.PurchaseValue);
        Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/v1/reports/finance?year=2042")).StatusCode);
    }
    [Fact]
    public async Task ReportReadAndExportAreIndependentAndBothRespectDepartmentScope()
    {
        using var exportOnly=await User(scope:"unit",denied:["reports.read"]);using var readOnly=await User(scope:"unit",denied:["reports.export"]);
        Assert.Equal(HttpStatusCode.Forbidden,(await exportOnly.GetAsync("/api/v1/reports/finance?year=2043")).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await exportOnly.GetAsync("/api/v1/reports/finance.csv?year=2043")).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await readOnly.GetAsync("/api/v1/reports/finance?year=2043")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await readOnly.GetAsync("/api/v1/reports/finance.csv?year=2043")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await exportOnly.GetAsync("/api/v1/reports/finance.csv?year=2043&departmentId=economics")).StatusCode);
        using var seeder=await User();await Receipt(seeder,2043,"it");await Receipt(seeder,2043,"economics");
        var scoped=await readOnly.GetFromJsonAsync<FinanceReport>("/api/v1/reports/finance?year=2043");Assert.Equal("it",Assert.Single(scoped!.Rows).DepartmentId);
        var csv=await exportOnly.GetStringAsync("/api/v1/reports/finance.csv?year=2043");Assert.Contains("Khoa CNTT",csv);Assert.DoesNotContain("Khoa Kinh tế",csv);
        using var admin=await factory.Login();Assert.Equal(HttpStatusCode.Forbidden,(await admin.GetAsync("/api/v1/reports/resources?year=2043")).StatusCode);
    }
    [Fact]
    public async Task CsvEscapesUntrustedDepartmentNameFromTheDatabase()
    {
        var admin=await factory.Admin();var dep=await Ok<DepartmentView>(admin.PostAsJsonAsync("/api/v1/departments",new SaveDepartment("F"+Key()[..8],"=SUM(1,1)",null,true,null)));
        using var c=await User();await Receipt(c,2048,dep.Id);
        var csv=await c.GetStringAsync($"/api/v1/reports/finance.csv?year=2048&departmentId={dep.Id}");Assert.Contains("\"'=SUM(1,1)\"",csv);
    }
    [Fact]
    public async Task AttachmentUploadReplayAndDownloadPersistExactBytesAndAudit()
    {
        using var c=await User();var entry=await Receipt(c);var key=Key();var a=await Ok<AttachmentView>(Upload(c,entry.Id,"chung-tu.pdf",key:key));
        Assert.Equal(a.Id,(await Ok<AttachmentView>(Upload(c,entry.Id,"chung-tu.pdf",key:key))).Id);
        var list=await c.GetFromJsonAsync<AttachmentView[]>($"/api/v1/finance/entries/{entry.Id}/attachments");Assert.Single(list!);
        var download=await c.GetAsync("/api/v1/voucher-attachments/"+a.Id);Assert.Equal(Pdf,await download.Content.ReadAsByteArrayAsync());Assert.Equal("attachment",download.Content.Headers.ContentDisposition!.DispositionType);Assert.Contains("sandbox",download.Headers.GetValues("Content-Security-Policy").Single());
        await using var scope=f.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();Assert.Equal(1,await db.AuditEvents.CountAsync(e=>e.TargetId==a.Id&&e.Action=="attachment.upload"));Assert.Equal(1,await db.AuditEvents.CountAsync(e=>e.TargetId==a.Id&&e.Action=="attachment.download"));
        Assert.Equal(HttpStatusCode.Conflict,(await Upload(c,entry.Id,"other.pdf",key:key)).StatusCode);
    }
    [Fact]
    public async Task AttachmentsRejectWrongScopeBadTypesLargeBodiesAndMissingCsrf()
    {
        using var c=await User();using var outsider=await User(department:"economics",scope:"unit");var entry=await Receipt(c);
        Assert.Equal(HttpStatusCode.Forbidden,(await outsider.GetAsync($"/api/v1/finance/entries/{entry.Id}/attachments")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await Upload(outsider,entry.Id,"x.pdf")).StatusCode);
        var a=await Ok<AttachmentView>(Upload(c,entry.Id,"x.pdf"));Assert.Equal(HttpStatusCode.Forbidden,(await outsider.GetAsync("/api/v1/voucher-attachments/"+a.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Upload(c,entry.Id,"x.html",Encoding.UTF8.GetBytes("<script>alert(1)</script>"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Upload(c,entry.Id,"x.pdf",Encoding.UTF8.GetBytes("<html>fake PDF</html>"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Upload(c,entry.Id,"../x.pdf")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Upload(c,entry.Id,"big.pdf",new byte[VoucherAttachmentService.MaxBytes+1])).StatusCode);
        c.DefaultRequestHeaders.Remove("X-CSRF-TOKEN");Assert.Equal(HttpStatusCode.BadRequest,(await Upload(c,entry.Id,"no-csrf.pdf")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,(await c.GetAsync("/VoucherAttachmentData/"+a.Id)).StatusCode);
    }
    [Fact]
    public async Task ConcurrentAttachmentQuotaKeepsMetadataAndContentAtomic()
    {
        using var c=await User();var entry=await Receipt(c,2046);
        for(var i=0;i<9;i++)await Ok<AttachmentView>(Upload(c,entry.Id,$"tep-{i}.pdf"));
        var responses=await Task.WhenAll(Upload(c,entry.Id,"last-1.pdf"),Upload(c,entry.Id,"last-2.pdf"));Assert.Single(responses,r=>r.StatusCode==HttpStatusCode.OK);Assert.Single(responses,r=>r.StatusCode==HttpStatusCode.Conflict);
        await using var scope=f.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();var ids=await db.VoucherAttachments.Where(a=>a.EntryId==entry.Id).Select(a=>a.Id).ToArrayAsync();Assert.Equal(10,ids.Length);Assert.Equal(10,await db.VoucherAttachmentData.CountAsync(a=>ids.Contains(a.AttachmentId)));
    }
    [Theory]
    [InlineData("=SUM(1,1)")][InlineData(" +cmd")][InlineData("\t@SUM(A1)")][InlineData("\uFEFF-1+2")]
    public void CsvNeutralizesFormulaPrefixesAndPreservesQuotedCells(string value)
    {
        var cell=CsvDocument.Cell(value);Assert.StartsWith("\"'",cell);
        Assert.Equal("\"a,\"\"b\"\"\r\nc\"",CsvDocument.Cell("a,\"b\"\r\nc"));Assert.Equal("\"\"",CsvDocument.Cell(null));
        Assert.Equal(new byte[]{239,187,191},CsvDocument.Create([new object?[]{value}])[..3]);
    }
}
