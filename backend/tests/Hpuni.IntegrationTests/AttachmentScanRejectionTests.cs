using System.Net;
using System.Net.Http.Json;
using System.Text;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Hpuni.IntegrationTests;

public class ScanRejectFactory:FinanceFactory
{
    public sealed class RejectScanner:IAttachmentScanner
    {
        public int Calls;
        public int Status=422;
        public Task Scan(ReadOnlyMemory<byte> content,CancellationToken ct){Interlocked.Increment(ref Calls);throw new AppException(Status,Status==422?"FILE_INFECTED":"FILE_SCAN_UNAVAILABLE","Rejected by test scanner.");}
    }
    public RejectScanner Scanner{get;}=new();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);builder.ConfigureTestServices(s=>s.Replace(ServiceDescriptor.Singleton<IAttachmentScanner>(Scanner)));
    }
}

[Collection("SQL Server")]
public class AttachmentScanRejectionTests(ScanRejectFactory factory):IClassFixture<ScanRejectFactory>
{
    static string Key()=>Guid.NewGuid().ToString("N");
    static Task<HttpResponseMessage> Upload(HttpClient c,string entry,string key)
    {
        var form=new MultipartFormDataContent();form.Add(new ByteArrayContent(Encoding.ASCII.GetBytes("%PDF-1.4\n%%EOF\n")),"file","fixture.pdf");
        var request=new HttpRequestMessage(HttpMethod.Post,$"/api/v1/finance/entries/{entry}/attachments"){Content=form};request.Headers.Add("Idempotency-Key",key);return c.SendAsync(request);
    }
    async Task<HttpClient> User(string department)
    {
        var admin=await factory.Admin();var user=await SqlFactory.Create(admin,"accounting",department);
        Assert.Equal(HttpStatusCode.OK,(await admin.PutAsJsonAsync($"/api/v1/users/{user.Id}/scope",new ConfigureScope("unit",[],[],user.Version))).StatusCode);return await factory.Login(user.UserName);
    }
    [Fact]
    public async Task InfectedUploadWritesNoMetadataBlobAuditOrIdempotencyAndUnauthorizedNeverScans()
    {
        using var c=await User("it");using var outsider=await User("economics");
        var record=new HttpRequestMessage(HttpMethod.Post,"/api/v1/finance/entries/receipt"){Content=JsonContent.Create(new RecordFinanceEntry("it",2026,"general",1,Key(),new(2026,1,2),"Scanner test"))};record.Headers.Add("Idempotency-Key",Key());
        var response=await c.SendAsync(record);Assert.Equal(HttpStatusCode.OK,response.StatusCode);var entry=(await response.Content.ReadFromJsonAsync<FinanceEntryView>())!;
        var key=Key();Assert.Equal(HttpStatusCode.UnprocessableEntity,(await Upload(c,entry.Id,key)).StatusCode);Assert.Equal(1,factory.Scanner.Calls);
        factory.Scanner.Status=503;Assert.Equal(HttpStatusCode.ServiceUnavailable,(await Upload(c,entry.Id,key)).StatusCode);Assert.Equal(2,factory.Scanner.Calls);
        Assert.Equal(HttpStatusCode.Forbidden,(await Upload(outsider,entry.Id,Key())).StatusCode);Assert.Equal(2,factory.Scanner.Calls);
        await using var scope=factory.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Empty(await db.VoucherAttachments.Where(x=>x.EntryId==entry.Id).ToArrayAsync());Assert.Equal(0,await db.VoucherAttachmentData.CountAsync());
        Assert.Equal(0,await db.AuditEvents.CountAsync(x=>x.Action=="attachment.upload"));Assert.False(await db.IdempotencyRecords.AnyAsync(x=>x.Key==key));
    }
}
