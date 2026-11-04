namespace Hpuni.Application;

public record ReportDetailColumn(string Label, bool Numeric = false);
public record ReportDetailRow(string Id, string[] Cells);
public record ReportDetailPage(string Kind, string Title, int Year, DateTimeOffset GeneratedAt,
    ReportDetailColumn[] Columns, ReportDetailRow[] Items, int Total, int Page, int PageSize, string Note);
