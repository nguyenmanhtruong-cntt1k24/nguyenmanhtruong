namespace Hpuni.Domain;

public class FiscalPeriod
{
    public string DepartmentId { get; set; } = "";
    public int Year { get; set; }
    public bool Closed { get; set; }
    public string Reason { get; set; } = "";
    public string ActorId { get; set; } = "";
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class BudgetAdjustment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DepartmentId { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "general";
    public decimal Amount { get; set; }
    public string Reason { get; set; } = "";
    public string Status { get; set; } = "pending";
    public string ActorId { get; set; } = "";
    public string? ReviewerId { get; set; }
    public string ReviewNote { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

// A correction is a separate, immutable record; the original voucher and files remain intact.
public class FinanceReversal
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = "";
    public string Reason { get; set; } = "";
    public string ActorId { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
