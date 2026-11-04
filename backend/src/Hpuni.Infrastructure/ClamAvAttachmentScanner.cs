using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;
using Hpuni.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hpuni.Infrastructure;

public class ClamAvAttachmentScanner(IOptions<AttachmentScanOptions> options,IHostEnvironment environment,
    ILogger<ClamAvAttachmentScanner> logger):IAttachmentScanner
{
    const int ResponseLimit=4096,ChunkSize=64*1024;
    int warned;
    static AppException Unavailable()=>new(503,"FILE_SCAN_UNAVAILABLE","Chưa thể kiểm tra an toàn tệp. Tệp chưa được lưu; hãy thử lại khi bộ quét sẵn sàng.");
    public async Task Scan(ReadOnlyMemory<byte> content,CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if(content.Length is <1 or >VoucherAttachmentService.MaxBytes)throw AppException.Invalid("Tệp phải từ 1 byte đến 2 MiB.");
        var o=options.Value;var mode=o.Mode?.Trim();var local=environment.IsDevelopment()||environment.IsEnvironment("Testing");
        if(local&&(string.Equals(mode,"EnvironmentDefault",StringComparison.OrdinalIgnoreCase)||string.Equals(mode,"SignatureOnly",StringComparison.OrdinalIgnoreCase))){
            if(Interlocked.Exchange(ref warned,1)==0)logger.LogWarning("Attachment scan is signature-only in {Environment}; this mode does not scan for malware.",environment.EnvironmentName);
            return;
        }
        if(!(string.Equals(mode,"ClamAv",StringComparison.OrdinalIgnoreCase)||string.Equals(mode,"EnvironmentDefault",StringComparison.OrdinalIgnoreCase))||
            string.IsNullOrWhiteSpace(o.Host)||o.Host.Length>253||o.Host.Any(char.IsControl)||o.Port is <1 or >65535||o.TimeoutMilliseconds is <100 or >60000)
            throw Unavailable();
        using var timeout=CancellationTokenSource.CreateLinkedTokenSource(ct);timeout.CancelAfter(o.TimeoutMilliseconds);
        try{
            using var client=new TcpClient();await client.ConnectAsync(o.Host,o.Port,timeout.Token);
            await using var stream=client.GetStream();await stream.WriteAsync("zINSTREAM\0"u8.ToArray(),timeout.Token);
            var header=new byte[4];
            for(var offset=0;offset<content.Length;offset+=ChunkSize){
                var length=Math.Min(ChunkSize,content.Length-offset);BinaryPrimitives.WriteInt32BigEndian(header,length);
                await stream.WriteAsync(header,timeout.Token);await stream.WriteAsync(content.Slice(offset,length),timeout.Token);
            }
            await stream.WriteAsync(new byte[4],timeout.Token);await stream.FlushAsync(timeout.Token);
            var response=new byte[ResponseLimit];var count=0;var one=new byte[1];
            while(true){
                if(await stream.ReadAsync(one,timeout.Token)==0)throw Unavailable();
                if(one[0]==0)break;
                if(count==response.Length||one[0] is <32 or >126)throw Unavailable();response[count++]=one[0];
            }
            var result=Encoding.ASCII.GetString(response,0,count);
            if(result=="stream: OK")return;
            if(result.StartsWith("stream: ",StringComparison.Ordinal)&&result.EndsWith(" FOUND",StringComparison.Ordinal))
                throw new AppException(422,"FILE_INFECTED","Bộ quét phát hiện tệp không an toàn. Tệp đã bị từ chối và chưa được lưu.");
            throw Unavailable();
        }catch(OperationCanceledException) when(!ct.IsCancellationRequested){throw Unavailable();}
        catch(SocketException){throw Unavailable();}
        catch(IOException){throw Unavailable();}
    }
}
