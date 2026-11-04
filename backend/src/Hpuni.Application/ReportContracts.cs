namespace Hpuni.Application;

public record FinanceReportRow(string DepartmentId,string DepartmentName,string Category,decimal Allocated,decimal Received,decimal Spent,decimal MaintenanceSpent)
{
    public decimal Available => Allocated-Spent;
}
public record PendingCostRow(string DepartmentId,string DepartmentName,decimal Pending,decimal MaintenancePending,decimal UnqueuedMaintenanceAllTime);
public record FinanceReport(int Year,string? Category,DateTimeOffset GeneratedAt,FinanceReportRow[] Rows,PendingCostRow[] PendingCosts);
public record ResourceReportRow(string DepartmentId,string DepartmentName)
{
    public long AssetGroups { get; init; }
    public long Owned { get; init; }
    public long Good { get; init; }
    public long Damaged { get; init; }
    public long Lost { get; init; }
    public long Outstanding { get; init; }
    public long OverdueLoans { get; init; }
    public long LoansStartingInYear { get; init; }
    public long MaintenancePlanned { get; init; }
    public long MaintenanceRunning { get; init; }
    public long ActiveRooms { get; init; }
    public long RoomRequestsStartingInYear { get; init; }
    public long ApprovedRoomRequestsStartingInYear { get; init; }
    public long AcquisitionProfiles { get; init; }
    public long MissingAcquisitionProfiles => AssetGroups - AcquisitionProfiles;
    public bool HistoricalDeclaredValueComplete => AssetGroups > 0 && AcquisitionProfiles == AssetGroups;
    public decimal? HistoricalDeclaredValue { get; init; }
    public decimal? PurchaseValue => null; // Current carrying value is unknown; declarations are historical group totals.
}
public record ResourceReport(int Year,DateTimeOffset GeneratedAt,ResourceReportRow[] Rows);
public record AttachmentView(string Id,string EntryId,string FileName,string ContentType,int Size,string Sha256,DateTimeOffset CreatedAt);
