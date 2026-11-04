namespace Hpuni.Domain;

public class MaintenanceOrder
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AssetGroupId { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public string Mode { get; set; } = "service";
    public int Quantity { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string Status { get; set; } = "planned";
    public string Content { get; set; } = "";
    public string Result { get; set; } = "";
    public string Note { get; set; } = "";
    public decimal Cost { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
public class InventorySession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public string Status { get; set; } = "open";
    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}
public class InventoryItem
{
    public string SessionId { get; set; } = "";
    public string AssetGroupId { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Expected { get; set; }
    public int ExpectedDamaged { get; set; }
    public int ExpectedLost { get; set; }
    public string AssetVersion { get; set; } = "";
    public int? Actual { get; set; }
    public string Condition { get; set; } = "";
    public string Note { get; set; } = "";
}
public class EquipmentUseLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LoanId { get; set; } = "";
    public bool Used { get; set; }
    public DateTimeOffset? ActualStart { get; set; }
    public DateTimeOffset? ActualEnd { get; set; }
    public int? Quantity { get; set; }
    public string Condition { get; set; } = "";
    public string Issue { get; set; } = "";
    public string Status { get; set; } = "submitted";
    public string ReviewNote { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
}
