using System.Data;
using System.Globalization;
using System.Net;
using System.Text;
using ClosedXML.Excel;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class ReportDetailsService(HpuniDbContext db, IAccessService access)
{
    public const int ExportLimit = 10000;
    static string Number(decimal value) => value.ToString("G29", CultureInfo.InvariantCulture);
    static string Date(DateTimeOffset value) => value.ToOffset(TimeSpan.FromHours(7)).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
    static IQueryable<T> Scope<T>(IQueryable<T> query, AccessProfile profile, string permission, string? department) where T : class
    {
        query = AssetOperationsService.Scoped(query.AsNoTracking(), profile, permission);
        return department == null ? query : query.Where(x => EF.Property<string>(x, "DepartmentId") == department);
    }
    public async Task<ReportDetailPage> Read(string actor, string kind, int year, string? department, string? category,
        int page, int pageSize, bool export, CancellationToken ct)
    {
        if (year is < 2000 or > 2100 || page is < 1 or > 100000 || pageSize is < 1 or > 100)
            throw AppException.Invalid("Năm hoặc phân trang báo cáo không hợp lệ.");
        if (kind is not ("finance" or "assets" or "loans" or "rooms")) throw AppException.Invalid("Loại báo cáo chi tiết không hợp lệ.");
        if (category != null && (kind != "finance" || !FinanceService.Categories.Contains(category))) throw AppException.Invalid("Khoản mục chỉ dùng cho báo cáo tài chính và phải hợp lệ.");
        var permission = export ? "reports.export" : "reports.read";
        var profile = await access.GetProfile(actor, ct);
        if (!AccessRules.HasPermission(profile, permission) || department != null && !AccessRules.Allows(profile, permission, department, null)) throw AppException.Forbidden();
        if (kind == "finance" && (!AccessRules.HasPermission(profile, "budget.read") || department != null && !AccessRules.Allows(profile, "budget.read", department, null))) throw AppException.Forbidden();
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
                var result = await ReadRows(profile, permission, kind, year, department, category, page, pageSize, export, ct);
                await tx.CommitAsync(ct); return result;
            }
            catch (SqlException e) when (e.Number == 1205 && attempt < 2) { await Task.Delay(50 * (attempt + 1), ct); }
        }
    }
    async Task<ReportDetailPage> ReadRows(AccessProfile profile, string permission, string kind, int year, string? department,
        string? category, int page, int pageSize, bool export, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var start = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.FromHours(7)); var end = start.AddYears(1);
        var skip = export ? 0 : (page - 1) * pageSize; var take = export ? ExportLimit : pageSize;
        void Limit(int total) { if (export && total > ExportLimit) throw AppException.Invalid("Báo cáo vượt 10.000 dòng. Hãy lọc theo đơn vị hoặc khoản mục trước khi xuất."); }
        ReportDetailPage Result(string title, ReportDetailColumn[] columns, ReportDetailRow[] rows, int total, string note) =>
            new(kind, title, year, now, columns, rows, total, export ? 1 : page, export ? total : pageSize, note);
        if (kind == "finance")
        {
            var source = AssetOperationsService.Scoped(Scope(db.FinanceEntries, profile, permission, department), profile, "budget.read").Where(x => x.Year == year);
            if (category != null) source = source.Where(x => x.Category == category);
            var total = await source.CountAsync(ct); Limit(total);
            var rows = await (from e in source join d in db.Departments on e.DepartmentId equals d.Id
                select new { Entry = e, Department = d.Name, Reversed = db.FinanceReversals.Any(r => r.EntryId == e.Id) })
                .OrderBy(x => x.Entry.Date).ThenBy(x => x.Entry.CreatedAt).ThenBy(x => x.Entry.Id).Skip(skip).Take(take).ToArrayAsync(ct);
            return Result("Sổ chứng từ chi tiết", [new("Ngày"), new("Số chứng từ"), new("Đơn vị"), new("Khoản mục"), new("Loại"), new("Số tiền VND", true), new("Trạng thái"), new("Nội dung"), new("Yêu cầu thanh toán")],
                rows.Select(x => new ReportDetailRow(x.Entry.Id, [x.Entry.Date.ToString("yyyy-MM-dd"), x.Entry.Voucher, x.Department, x.Entry.Category,
                    x.Entry.Kind, Number(x.Entry.Amount), x.Reversed ? "Đã đảo" : "Có hiệu lực", x.Entry.Content, x.Entry.PaymentRequestId ?? ""])).ToArray(), total,
                "Giữ nguyên chứng từ gốc, kể cả chứng từ đã đảo. Dòng đã đảo không tính vào số thu/chi có hiệu lực. Báo cáo này không cộng số tiền của các trang thành số dư ngân sách.");
        }
        if (kind == "assets")
        {
            var source = Scope(db.AssetGroups, profile, permission, department); var total = await source.CountAsync(ct); Limit(total);
            var rows = await (from a in source join d in db.Departments on a.DepartmentId equals d.Id
                join p in db.Set<FixedAssetProfile>() on a.Id equals p.AssetGroupId into profiles from p in profiles.DefaultIfEmpty()
                select new { Asset = a, Department = d.Name, AcquisitionValue = p == null ? (decimal?)null : p.AcquisitionValue, AcquisitionDate = p == null ? (DateOnly?)null : p.AcquisitionDate })
                .OrderBy(x => x.Asset.Code).ThenBy(x => x.Asset.Id).Skip(skip).Take(take).ToArrayAsync(ct);
            return Result("Danh mục tài sản hiện tại", [new("Mã"), new("Tài sản"), new("Đơn vị sở hữu"), new("Đơn vị tính"), new("Sở hữu", true), new("Tốt", true), new("Hỏng", true), new("Mất", true), new("Trạng thái"), new("Nguyên giá nhóm kê khai lịch sử (VND)", true), new("Ngày mua kê khai")],
                rows.Select(x => new ReportDetailRow(x.Asset.Id, [x.Asset.Code, x.Asset.Name, x.Department, x.Asset.Unit, Number(x.Asset.Total), Number(x.Asset.Total - x.Asset.Damaged - x.Asset.Lost), Number(x.Asset.Damaged), Number(x.Asset.Lost), x.Asset.Active ? "Hoạt động" : "Ngừng", x.AcquisitionValue.HasValue ? Number(x.AcquisitionValue.Value) : "", x.AcquisitionDate?.ToString("yyyy-MM-dd") ?? ""])).ToArray(), total,
                "Tồn hiện tại tại thời điểm lấy dữ liệu, không phải tồn cuối năm được chọn. Số tốt bao gồm thiết bị đang mượn hoặc bảo trì, không phải khả dụng để đặt ngay. Nguyên giá kê khai lịch sử cho cả nhóm, không tự giảm sau thanh lý và không phải giá trị còn lại; trống là chưa kê khai. Chưa tính khấu hao.");
        }
        if (kind == "loans")
        {
            var assets = Scope(db.AssetGroups, profile, permission, department);
            var source = from l in db.EquipmentLoans.AsNoTracking() join a in assets on l.AssetGroupId equals a.Id join d in db.Departments on a.DepartmentId equals d.Id
                where l.Start >= start && l.Start < end select new { Loan = l, Asset = a.Name, Department = d.Name };
            var total = await source.CountAsync(ct); Limit(total);
            var rows = await source.OrderBy(x => x.Loan.Start).ThenBy(x => x.Loan.Id).Skip(skip).Take(take).ToArrayAsync(ct);
            return Result("Phiếu mượn bắt đầu trong năm", [new("Thiết bị"), new("Đơn vị sở hữu"), new("Bắt đầu (VN)"), new("Hạn trả (VN)"), new("Mượn", true), new("Đã trả", true), new("Trạng thái"), new("Quá hạn")],
                rows.Select(x => new ReportDetailRow(x.Loan.Id, [x.Asset, x.Department, Date(x.Loan.Start), Date(x.Loan.End), Number(x.Loan.Quantity), Number(x.Loan.Returned), x.Loan.Status,
                    (x.Loan.Status is "ready" or "receipt_disputed" or "borrowed" or "return_pending") && x.Loan.End < now && x.Loan.Quantity > x.Loan.Returned ? "Có" : "Không"])).ToArray(), total,
                "Năm tính theo lúc bắt đầu phiếu ở múi giờ Việt Nam, gồm mọi trạng thái. Phạm vi và đơn vị thống kê là đơn vị sở hữu thiết bị.");
        }
        {
            var rooms = Scope(db.Rooms, profile, permission, department);
            var source = from b in db.RoomRequests.AsNoTracking() join r in rooms on b.RoomId equals r.Id join d in db.Departments on r.DepartmentId equals d.Id
                where b.Start >= start && b.Start < end select new { Booking = b, Room = r.Name, Department = d.Name };
            var total = await source.CountAsync(ct); Limit(total);
            var rows = await source.OrderBy(x => x.Booking.Start).ThenBy(x => x.Booking.Id).Skip(skip).Take(take).ToArrayAsync(ct);
            return Result("Phiếu đặt phòng bắt đầu trong năm", [new("Phòng"), new("Đơn vị sở hữu"), new("Bắt đầu (VN)"), new("Kết thúc (VN)"), new("Số người", true), new("Trạng thái"), new("Mục đích")],
                rows.Select(x => new ReportDetailRow(x.Booking.Id, [x.Room, x.Department, Date(x.Booking.Start), Date(x.Booking.End), Number(x.Booking.Attendees), x.Booking.Status, x.Booking.Purpose])).ToArray(), total,
                "Năm tính theo lúc bắt đầu phiếu ở múi giờ Việt Nam, gồm mọi trạng thái. Phạm vi và đơn vị thống kê là đơn vị sở hữu phòng.");
        }
    }
    public static byte[] Csv(ReportDetailPage report) => CsvDocument.Create(new[] { report.Columns.Select(x => (object?)x.Label).ToArray() }
        .Concat(report.Items.Select(x => x.Cells.Select(c => (object?)c).ToArray())));
    public static byte[] Excel(ReportDetailPage report)
    {
        using var workbook = new XLWorkbook(); var sheet = workbook.Worksheets.Add("Bao cao");
        sheet.Cell(1, 1).Value = report.Title; sheet.Cell(2, 1).Value = report.Note;
        sheet.Cell(3, 1).Value = "Lấy dữ liệu: " + Date(report.GeneratedAt) + " (VN) · " + report.Total + " dòng";
        for (var i = 0; i < report.Columns.Length; i++) sheet.Cell(5, i + 1).Value = report.Columns[i].Label;
        for (var r = 0; r < report.Items.Length; r++)
            for (var c = 0; c < report.Columns.Length; c++)
            {
                var cell = sheet.Cell(r + 6, c + 1); var value = report.Items[r].Cells[c];
                if (report.Columns[c].Numeric && decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number)) cell.Value = (double)number;
                else cell.Value = value; // String value stays text; never assign FormulaA1 from untrusted data.
            }
        sheet.Range(5, 1, 5, report.Columns.Length).Style.Font.Bold = true;
        sheet.Range(5, 1, 5, report.Columns.Length).Style.Fill.BackgroundColor = XLColor.LightBlue;
        sheet.SheetView.FreezeRows(5); sheet.Range(5, 1, Math.Max(5, report.Items.Length + 5), report.Columns.Length).SetAutoFilter();
        sheet.Columns(1, report.Columns.Length).Width = 24; sheet.Row(2).Height = 45; sheet.Cell(2, 1).Style.Alignment.WrapText = true;
        sheet.Range(2, 1, 2, report.Columns.Length).Merge();
        using var stream = new MemoryStream(); workbook.SaveAs(stream); return stream.ToArray();
    }
    public static string PrintHtml(ReportDetailPage report)
    {
        static string E(string value) => WebUtility.HtmlEncode(value);
        var html = new StringBuilder("<!doctype html><html lang=\"vi\"><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>")
            .Append(E(report.Title)).Append("</title><style>body{font:12px Arial,sans-serif;margin:24px;color:#172033}table{border-collapse:collapse;width:100%}th,td{border:1px solid #aaa;padding:6px;text-align:left;overflow-wrap:anywhere}thead{display:table-header-group}tr{break-inside:avoid}h1{font-size:22px}@page{size:A4 landscape;margin:12mm}@media print{.screen{display:none}}</style><body><h1>")
            .Append(E(report.Title)).Append("</h1><p>Năm ").Append(report.Year).Append(" · ").Append(report.Total).Append(" dòng · ").Append(E(Date(report.GeneratedAt))).Append(" (VN)</p><p>").Append(E(report.Note))
            .Append("</p><p class=\"screen\">Dùng Ctrl+P hoặc lệnh In của trình duyệt, chọn Lưu dưới dạng PDF. Đây là báo cáo nội bộ, chưa phải biểu mẫu kế toán có chữ ký.</p><table><thead><tr>");
        foreach (var column in report.Columns) html.Append("<th>").Append(E(column.Label)).Append("</th>");
        html.Append("</tr></thead><tbody>");
        foreach (var row in report.Items) { html.Append("<tr>"); foreach (var cell in row.Cells) html.Append("<td>").Append(E(cell)).Append("</td>"); html.Append("</tr>"); }
        return html.Append("</tbody></table></body></html>").ToString();
    }
}
