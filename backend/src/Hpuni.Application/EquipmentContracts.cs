namespace Hpuni.Application;

public record SaveAssetGroup(string Code, string Name, string Category, string Unit, string DepartmentId,
    bool Active, string? Version);
public record ReceiveStock(int Quantity, string Version, string? Note);
public record AssetGroupView(string Id, string Code, string Name, string Category, string Unit,
    string DepartmentId, int Total, int Damaged, int Lost, int Outstanding, int AvailableNow, bool Active, string Version, int MaintenanceBlockedNow = 0);
public record SaveEquipmentLoan(string AssetGroupId, DateTimeOffset Start, DateTimeOffset End, int Quantity,
    string? Purpose, string? Version);
public record EquipmentCommand(string Version, string? Note, int? Quantity = null);
public record ReturnEquipment(string Version, int Good, int Damaged, int Lost, string? Note);
public record AcceptEquipmentReturn(string Version, string ReturnId, int Good, int Damaged, int Lost, string? Note);
public record EquipmentReturnView(string Id, int Good, int Damaged, int Lost, int? AcceptedGood,
    int? AcceptedDamaged, int? AcceptedLost, string Note, string AcceptanceNote, DateTimeOffset At, DateTimeOffset? AcceptedAt);
public record EquipmentLoanView(string Id, string AssetGroupId, string AssetCode, string AssetName,
    string OwnerId, string DepartmentId, DateTimeOffset Start, DateTimeOffset End, int Quantity,
    int Returned, string Purpose, string Status, string Note, int? ReportedReceived, bool Overdue,
    string Version, EquipmentReturnView[] Returns);
