namespace Hpuni.Infrastructure;

public class AttachmentScanOptions
{
    // EnvironmentDefault permits signature-only ONLY in Development/Testing.
    public string Mode { get; set; }="EnvironmentDefault";
    public string Host { get; set; }="";
    public int Port { get; set; }=3310;
    public int TimeoutMilliseconds { get; set; }=10000;
}
