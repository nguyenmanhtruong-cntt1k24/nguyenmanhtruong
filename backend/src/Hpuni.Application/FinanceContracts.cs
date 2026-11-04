namespace Hpuni.Application;

public record ProposalLine(string Name, int Quantity, decimal Price);
public record SaveProposal(string DepartmentId, int Year, string Category, string Title, string? Purpose, ProposalLine[] Lines, string? Version = null);
public record ProposalCommand(string Version, string? Note = null);
public record ProposalView(string Id, string DepartmentId, int Year, string Category, string Title, string Purpose, string OwnerId, string Status, decimal Amount, ProposalLine[] Lines, string ReviewNote, string? ReviewerId, string Version);
public record CreatePaymentRequest(string DepartmentId, int Year, decimal Amount, string Content);
public record ImportMaintenanceCost(int Year);
public record RecordFinanceEntry(string DepartmentId, int Year, string Category, decimal Amount, string Voucher, DateOnly Date, string Content, string? PaymentRequestId = null, string? Version = null);
public record PaymentView(string Id, string DepartmentId, int Year, decimal Amount, string Content, string? MaintenanceId, string Status, string Version)
{
    public decimal Paid { get; init; }
    public decimal Remaining => Amount - Paid;
}
public record FinanceEntryView(string Id, string DepartmentId, int Year, string Category, string Kind, decimal Amount, string Voucher, DateOnly Date, string Content, string? PaymentRequestId);
public record BudgetBalance(string DepartmentId, int Year, string Category, decimal Allocated, decimal Spent, decimal Available);
public record FinanceSummary(BudgetBalance[] Balances, decimal Received);
