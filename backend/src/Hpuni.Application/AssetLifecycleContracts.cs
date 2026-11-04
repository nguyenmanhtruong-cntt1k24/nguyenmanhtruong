namespace Hpuni.Application;

public record SaveFixedAssetProfile(decimal AcquisitionValue, DateOnly AcquisitionDate, string Source, string Reference, string? Note, string? Version = null);
public record FixedAssetProfileView(string AssetGroupId, decimal AcquisitionValue, DateOnly AcquisitionDate, string Source, string Reference, string Note, DateTimeOffset UpdatedAt, string Version);
public record CreateAssetDisposal(string AssetGroupId, int Good, int Damaged, string Reason, string Reference);
public record ReviewAssetDisposal(string Version, string Note);
public record AssetDisposalView(string Id, string AssetGroupId, string DepartmentId, int Good, int Damaged, string Reason, string Reference,
    string Status, string ActorId, string? ReviewerId, string ReviewNote, DateTimeOffset CreatedAt, DateTimeOffset? ReviewedAt, string Version)
{
    public string AssetCode { get; init; } = "";
    public string AssetName { get; init; } = "";
}
