namespace Hpuni.Application;

public record ChangeFiscalPeriod(string DepartmentId, int Year, bool Closed, string Reason, string? Version = null);
public record FiscalPeriodView(string DepartmentId, int Year, bool Closed, string Reason, DateTimeOffset? ChangedAt, string? Version);
public record CreateBudgetAdjustment(string DepartmentId, int Year, string Category, decimal Amount, string Reason);
public record ReviewBudgetAdjustment(string Version, bool Approve, string Note);
public record BudgetAdjustmentView(string Id, string DepartmentId, int Year, string Category, decimal Amount, string Reason, string Status, string ActorId, string? ReviewerId, string ReviewNote, string Version);
public record ReverseFinanceEntry(string Reason);
public record FinanceReversalView(string Id, string EntryId, string Reason, string ActorId, DateTimeOffset CreatedAt);
