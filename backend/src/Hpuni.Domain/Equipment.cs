namespace Hpuni.Domain;

// A group is the single quantity owner for both the asset register and lending catalogue.
public class AssetGroup
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Unit { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public int Total { get; set; }
    public int Damaged { get; set; }
    public int Lost { get; set; }
    public bool Active { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public class EquipmentLoan
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AssetGroupId { get; set; } = "";
    public AssetGroup Asset { get; set; } = null!;
    public string OwnerId { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int Quantity { get; set; }
    public int Returned { get; set; }
    public string Purpose { get; set; } = "";
    public string Status { get; set; } = "draft";
    public string Note { get; set; } = "";
    public DateTimeOffset? HandedAt { get; set; }
    public int? ReportedReceived { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class EquipmentReturn
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LoanId { get; set; } = "";
    public int Good { get; set; }
    public int Damaged { get; set; }
    public int Lost { get; set; }
    public int? AcceptedGood { get; set; }
    public int? AcceptedDamaged { get; set; }
    public int? AcceptedLost { get; set; }
    public string Note { get; set; } = "";
    public string AcceptanceNote { get; set; } = "";
    public string? AcceptedBy { get; set; }
    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AcceptedAt { get; set; }
}

public class EquipmentMovement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AssetGroupId { get; set; } = "";
    public string? LoanId { get; set; }
    public string Kind { get; set; } = "";
    public int Quantity { get; set; }
    public string ActorId { get; set; } = "";
    public string Note { get; set; } = "";
    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
}

public record QuantitySpan(DateTimeOffset Start, DateTimeOffset End, int Quantity);

public static class EquipmentCapacity
{
    // Clip before sweeping; merge equal instants to preserve half-open intervals.
    public static long Peak(IEnumerable<QuantitySpan> spans, DateTimeOffset start, DateTimeOffset end)
    {
        var events = spans.Where(s => s.Quantity > 0 && s.Start < end && s.End > start)
            .SelectMany(s => new[] { (At: s.Start > start ? s.Start : start, Delta: (long)s.Quantity),
                (At: s.End < end ? s.End : end, Delta: -(long)s.Quantity) })
            .GroupBy(x => x.At).OrderBy(x => x.Key);
        long used = 0, peak = 0;
        foreach (var e in events) { used += e.Sum(x => x.Delta); peak = Math.Max(peak, used); }
        return peak;
    }
}
