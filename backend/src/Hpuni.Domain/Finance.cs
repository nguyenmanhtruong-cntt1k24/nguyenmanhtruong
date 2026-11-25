namespace Hpuni.Domain;

public class BudgetProposal
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DepartmentId { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "general";
    public string Title { get; set; } = "";
    public string Purpose { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string Status { get; set; } = "draft";
    public decimal Amount { get; set; }
    public string LinesJson { get; set; } = "[]";
    public string ReviewNote { get; set; } = "";
    public string? ReviewerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class BudgetAllocation
{
    public string ProposalId { get; set; } = "";
    public decimal Amount { get; set; }
}

public class PaymentRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DepartmentId { get; set; } = "";
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public string Content { get; set; } = "";
    public string? MaintenanceId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class FinanceEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DepartmentId { get; set; } = "";
    public int Year { get; set; }
    public string Category { get; set; } = "general";
    public string Kind { get; set; } = "expense";
    public decimal Amount { get; set; }
    public string Voucher { get; set; } = "";
    public string NormalizedVoucher { get; set; } = "";
    public DateOnly Date { get; set; }
    public string Content { get; set; } = "";
    public string ActorId { get; set; } = "";
    public string? PaymentRequestId { get; set; }
    public string? FundingProposalId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
