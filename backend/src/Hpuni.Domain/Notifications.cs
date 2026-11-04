namespace Hpuni.Domain;

// Acknowledgements contain no source text. Visibility is re-evaluated from the live source.
public class NotificationReadState
{
    public string ActorId { get; set; } = "";
    public string SourceKey { get; set; } = "";
    public byte[] SourceVersion { get; set; } = [];
    public DateTimeOffset ReadAt { get; set; } = DateTimeOffset.UtcNow;
}
