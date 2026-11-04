namespace Hpuni.Application;

public record PlanMaintenance(string AssetGroupId, string Mode, int Quantity, DateTimeOffset Start, DateTimeOffset End, string? Content);
public record FinishMaintenance(string Version, string Result, decimal Cost, string? Note);
public record MaintenanceView(string Id, string AssetGroupId, string DepartmentId, string Mode, int Quantity,
    DateTimeOffset Start, DateTimeOffset End, string Status, string Content, string Result, string Note, decimal Cost, string Version,
    string? PaymentRequestId = null, string? PaymentStatus = null, string? Voucher = null, string? FinanceEntryId = null);
public record CreateInventory(string Name, string DepartmentId);
public record CountInventoryItem(string AssetGroupId, int? Actual, string Condition, string? Note);
public record SaveInventory(string Version, CountInventoryItem[] Items, bool Complete);
public record InventoryItemView(string AssetGroupId, string Code, string Name, int Expected, int ExpectedDamaged,
    int ExpectedLost, string AssetVersion, int? Actual, string Condition, string Note);
public record InventoryView(string Id, string Name, string DepartmentId, string Status, DateTimeOffset At,
    string Version, InventoryItemView[] Items);
public record SaveEquipmentLog(bool Used, DateTimeOffset? ActualStart, DateTimeOffset? ActualEnd, int? Quantity,
    string? Condition, string? Issue, string? Version);
public record EquipmentLogView(string Id, string LoanId, bool Used, DateTimeOffset? ActualStart, DateTimeOffset? ActualEnd,
    int? Quantity, string Condition, string Issue, string Status, string ReviewNote, string Version);
