namespace Hpuni.Domain;

// Historical acquisition declaration for an aggregate group; not a per-item book value.
public class FixedAssetProfile
{
    public string AssetGroupId { get; set; } = "";
    public decimal AcquisitionValue { get; set; }
    public DateOnly AcquisitionDate { get; set; }
    public string Source { get; set; } = "";
    public string Reference { get; set; } = "";
    public string Note { get; set; } = "";
    public string ActorId { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class AssetDisposal
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AssetGroupId { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public int Good { get; set; }
    public int Damaged { get; set; }
    public string Reason { get; set; } = "";
    public string Reference { get; set; } = "";
    public string Status { get; set; } = "pending";
    public string ActorId { get; set; } = "";
    public string? ReviewerId { get; set; }
    public string ReviewNote { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReviewedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
