namespace Hpuni.Domain;

public class VoucherAttachment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "";
    public int Size { get; set; }
    public string Sha256 { get; set; } = "";
    public string ActorId { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public class VoucherAttachmentData
{
    public string AttachmentId { get; set; } = "";
    public byte[] Content { get; set; } = [];
}
