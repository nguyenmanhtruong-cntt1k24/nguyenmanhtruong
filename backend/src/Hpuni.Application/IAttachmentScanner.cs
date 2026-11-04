namespace Hpuni.Application;

public interface IAttachmentScanner
{
    Task Scan(ReadOnlyMemory<byte> content,CancellationToken ct);
}
