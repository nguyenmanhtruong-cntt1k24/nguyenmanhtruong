using System.Security.Cryptography;
using System.Text;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class VoucherAttachmentService(HpuniDbContext db,FinanceService finance,AdminService admin,IdempotentCommands commands,IAttachmentScanner scanner)
{
    public const int MaxBytes=2*1024*1024;
    public const int MaxFiles=10;
    public static AttachmentView View(VoucherAttachment a)=>new(a.Id,a.EntryId,a.FileName,a.ContentType,a.Size,a.Sha256,a.CreatedAt);
    async Task<FinanceEntry> Entry(string actor,string id,string permission,CancellationToken ct)
    {
        var e=await db.FinanceEntries.AsNoTracking().SingleOrDefaultAsync(e=>e.Id==id,ct)??throw AppException.Missing();
        await finance.Demand(actor,permission,e.DepartmentId,ct);return e;
    }
    public async Task<AttachmentView[]> List(string actor,string entry,CancellationToken ct)
    {
        var e=await Entry(actor,entry,"budget.read",ct);
        return (await db.VoucherAttachments.AsNoTracking().Where(x=>x.EntryId==e.Id).OrderBy(x=>x.CreatedAt).ThenBy(x=>x.Id).ToArrayAsync(ct)).Select(View).ToArray();
    }
    public static (string Name,string Type) Validate(string fileName,byte[] bytes)
    {
        var name=fileName.Trim();
        if(name.Length is <1 or >150||name.Any(c=>char.IsControl(c)||"/\\:<>|\"?*".Contains(c)))throw AppException.Invalid("Tên tệp tối đa 150 ký tự, không chứa đường dẫn hoặc ký tự điều khiển.");
        if(bytes.Length is <1 or >MaxBytes)throw AppException.Invalid("Tệp phải từ 1 byte đến 2 MiB.");
        var extension=Path.GetExtension(name).ToLowerInvariant();string? type=null;
        if(extension==".pdf"&&bytes.Length>=12&&bytes.AsSpan(0,5).SequenceEqual("%PDF-"u8)&&Encoding.ASCII.GetString(bytes,Math.Max(0,bytes.Length-1024),Math.Min(1024,bytes.Length)).Contains("%%EOF",StringComparison.Ordinal))type="application/pdf";
        if(extension==".png"&&bytes.Length>=20&&bytes.AsSpan(0,8).SequenceEqual(new byte[]{137,80,78,71,13,10,26,10})&&bytes.AsSpan(bytes.Length-8,4).SequenceEqual("IEND"u8))type="image/png";
        if(extension is ".jpg" or ".jpeg"&&bytes.Length>=4&&bytes[0]==255&&bytes[1]==216&&bytes[2]==255&&bytes[^2]==255&&bytes[^1]==217)type="image/jpeg";
        if(type==null)throw AppException.Invalid("Chỉ nhận PDF, PNG hoặc JPEG có chữ ký nội dung phù hợp. Tệp không được trống hoặc bị cắt.");
        return (name,type);
    }
    public async Task<AttachmentView> Upload(string actor,string entry,string key,string fileName,byte[] bytes,CancellationToken ct)
    {
        var(name,type)=Validate(fileName,bytes);var hash=Convert.ToHexString(SHA256.HashData(bytes));
        // Authorize before sending bytes to the configured internal scanner. Do not hold SQL locks while scanning.
        await Entry(actor,entry,"transactions.write",ct);await scanner.Scan(bytes,ct);
        return await commands.Run(actor,key,"attachment.upload",new{entry,name,type,hash,size=bytes.Length},async()=>{await Entry(actor,entry,"transactions.write",ct);},async()=>{
            var e=await Entry(actor,entry,"transactions.write",ct);await TransactionLocks.Acquire(db,"attachments:"+e.Id,ct);
            if(await db.VoucherAttachments.CountAsync(x=>x.EntryId==e.Id,ct)>=MaxFiles)throw new AppException(409,"ATTACHMENT_LIMIT","Mỗi chứng từ tối đa 10 tệp; không tự xóa hoặc ghi đè tệp cũ.");
            var a=new VoucherAttachment{EntryId=e.Id,FileName=name,ContentType=type,Size=bytes.Length,Sha256=hash,ActorId=actor};
            db.VoucherAttachments.Add(a);db.VoucherAttachmentData.Add(new(){AttachmentId=a.Id,Content=bytes});
            admin.Audit(actor,"attachment.upload",a.Id,new{a.EntryId,a.FileName,a.Size,a.Sha256});await db.SaveChangesAsync(ct);return View(a);
        },ct);
    }
    public async Task<(AttachmentView Metadata,byte[] Content)> Download(string actor,string id,CancellationToken ct)
    {
        var a=await db.VoucherAttachments.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id,ct)??throw AppException.Missing();
        await Entry(actor,a.EntryId,"budget.read",ct);
        var content=await db.VoucherAttachmentData.AsNoTracking().Where(x=>x.AttachmentId==a.Id).Select(x=>x.Content).SingleAsync(ct);
        if(content.Length!=a.Size||Convert.ToHexString(SHA256.HashData(content))!=a.Sha256)throw new AppException(409,"FILE_INTEGRITY","Nội dung tệp không khớp thông tin lưu trữ.");
        admin.Audit(actor,"attachment.download",a.Id,new{a.EntryId,a.Size,a.Sha256});await db.SaveChangesAsync(ct);
        return(View(a),content);
    }
}
