using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Hpuni.IntegrationTests;

public class AttachmentScannerTests
{
    sealed class TestEnvironment(string name):IHostEnvironment
    {
        public string EnvironmentName{get;set;}=name;
        public string ApplicationName{get;set;}="scanner-tests";
        public string ContentRootPath{get;set;}="";
        public IFileProvider ContentRootFileProvider{get;set;}=new NullFileProvider();
    }
    static ClamAvAttachmentScanner Scanner(int port=3310,int timeout=1000,string environment="Production",string mode="ClamAv",string host="127.0.0.1")=>
        new(Options.Create(new AttachmentScanOptions{Host=host,Port=port,TimeoutMilliseconds=timeout,Mode=mode}),new TestEnvironment(environment),NullLogger<ClamAvAttachmentScanner>.Instance);
    static async Task WithServer(byte[] payload,string response,Func<int,Task> test,int delay=0,bool terminate=true)
    {
        using var listener=new TcpListener(IPAddress.Loopback,0);listener.Start();var port=((IPEndPoint)listener.LocalEndpoint).Port;
        var serving=Task.Run(async()=>{
            using var stop=new CancellationTokenSource(TimeSpan.FromSeconds(5));using var client=await listener.AcceptTcpClientAsync(stop.Token);await using var stream=client.GetStream();
            var command=new byte[10];await stream.ReadExactlyAsync(command,stop.Token);Assert.Equal("zINSTREAM\0",Encoding.ASCII.GetString(command));
            using var content=new MemoryStream();var size=new byte[4];
            while(true){await stream.ReadExactlyAsync(size,stop.Token);var length=BinaryPrimitives.ReadInt32BigEndian(size);if(length==0)break;Assert.InRange(length,1,64*1024);var block=new byte[length];await stream.ReadExactlyAsync(block,stop.Token);content.Write(block);}
            Assert.Equal(payload,content.ToArray());if(delay>0)await Task.Delay(delay,stop.Token);
            var bytes=Encoding.ASCII.GetBytes(response+(terminate?"\0":""));
            try{for(var i=0;i<bytes.Length;i+=3)await stream.WriteAsync(bytes.AsMemory(i,Math.Min(3,bytes.Length-i)),stop.Token);}catch(IOException)when(delay>0||response.Length>4096){}
        });
        await test(port);await serving;
    }
    [Fact]
    public async Task InstreamSendsNetworkByteOrderChunksAndAcceptsOnlyCleanResult()
    {
        var bytes=Enumerable.Range(0,150000).Select(x=>(byte)(x%251)).ToArray();await WithServer(bytes,"stream: OK",async port=>await Scanner(port).Scan(bytes,default));
    }
    [Theory]
    [InlineData("stream: Eicar-Test-Signature FOUND",422,"FILE_INFECTED")]
    [InlineData("INSTREAM size limit exceeded. ERROR",503,"FILE_SCAN_UNAVAILABLE")]
    [InlineData("not a scan response",503,"FILE_SCAN_UNAVAILABLE")]
    [InlineData("stream: OK extra",503,"FILE_SCAN_UNAVAILABLE")]
    public async Task InfectionErrorsAndAmbiguousResponsesAreRejected(string response,int status,string code)
    {
        var bytes="test content"u8.ToArray();await WithServer(bytes,response,async port=>{var error=await Assert.ThrowsAsync<AppException>(()=>Scanner(port).Scan(bytes,default));Assert.Equal(status,error.Status);Assert.Equal(code,error.Code);});
    }
    [Fact]
    public async Task ResponseMustBeTerminatedBoundedAndWithinDeadline()
    {
        var bytes="test"u8.ToArray();
        foreach(var args in new[]{(Response:"stream: OK",Delay:0,Terminate:false),(Response:new string('x',4097),Delay:0,Terminate:true),(Response:"stream: OK",Delay:400,Terminate:true)})
            await WithServer(bytes,args.Response,async port=>{var error=await Assert.ThrowsAsync<AppException>(()=>Scanner(port,100).Scan(bytes,default));Assert.Equal("FILE_SCAN_UNAVAILABLE",error.Code);},args.Delay,args.Terminate);
    }
    [Fact]
    public async Task MissingConfigurationProductionBypassAndConnectionFailureFailClosed()
    {
        var bytes="test"u8.ToArray();
        foreach(var scanner in new[]{Scanner(host:""),Scanner(mode:"SignatureOnly"),Scanner(mode:"unknown"),Scanner(timeout:1),Scanner(port:0)})
            Assert.Equal("FILE_SCAN_UNAVAILABLE",(await Assert.ThrowsAsync<AppException>(()=>scanner.Scan(bytes,default))).Code);
        using var listener=new TcpListener(IPAddress.Loopback,0);listener.Start();var port=((IPEndPoint)listener.LocalEndpoint).Port;listener.Stop();
        Assert.Equal("FILE_SCAN_UNAVAILABLE",(await Assert.ThrowsAsync<AppException>(()=>Scanner(port).Scan(bytes,default))).Code);
    }
    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public async Task OnlyLocalEnvironmentsAllowSignatureOnlyDefault(string environment)
    {
        await Scanner(environment:environment,mode:"EnvironmentDefault",host:"").Scan("test"u8.ToArray(),default);
        await Scanner(environment:environment,mode:"SignatureOnly",host:"").Scan("test"u8.ToArray(),default);
        // Explicit ClamAv never falls back, including a local environment.
        await Assert.ThrowsAsync<AppException>(()=>Scanner(environment:environment,mode:"ClamAv",host:"").Scan("test"u8.ToArray(),default));
    }
    [Fact]
    public async Task CallerCancellationRemainsCancellation()
    {
        using var cancel=new CancellationTokenSource();cancel.Cancel();await Assert.ThrowsAnyAsync<OperationCanceledException>(()=>Scanner().Scan("test"u8.ToArray(),cancel.Token));
    }
}
