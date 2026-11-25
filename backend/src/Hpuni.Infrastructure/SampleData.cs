using System.Text.Json;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

// Explicit maintenance command only. Never runs at startup or from an HTTP request.
// Stable keys and one transaction make a repeated run safe without overwriting user data.
public static class SampleData
{
    public static async Task<object> Run(HpuniDbContext db, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "sample-data:v1", ct);
        var users = await db.Users.Where(u => u.Active).OrderBy(u => u.Id).ToListAsync(ct);
        var actor = users.FirstOrDefault(u => u.UserName == "admin.hpuni") ?? throw new InvalidOperationException("Cần tài khoản quản trị hiện có.");
        var departments = await db.Departments.Where(d => d.Active).OrderBy(d => d.Id).ToListAsync(ct);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "Asia/Ho_Chi_Minh").DateTime);
        DateTimeOffset At(int offset, int hour) => new(today.AddDays(offset).ToDateTime(new TimeOnly(hour, 0)), TimeSpan.FromHours(7));
        var added = new Dictionary<string, int>();
        async Task Add<T>(T entity, params object[] keys) where T : class
        {
            if (await db.Set<T>().FindAsync(keys, ct) != null) return;
            db.Add(entity);
            var name = typeof(T).Name;
            added[name] = added.GetValueOrDefault(name) + 1;
        }
        foreach (var d in departments)
        {
            var prefix = "sample-v1-" + d.Id;
            // Dedicated sample catalogue keeps all quantities and financial values separate from real records.
            var roomId = prefix + "-room";
            await Add(new Room { Id = roomId, Code = "MAU-" + d.Code + "-P01", Name = "[MẪU] Phòng học " + d.Name, Type = "Phòng học", DepartmentId = d.Id, Capacity = 60 }, roomId);
            var assetId = prefix + "-projector";
            await Add(new AssetGroup { Id = assetId, Code = "MAU-" + d.Code + "-MC", Name = "[MẪU] Máy chiếu phục vụ giảng dạy", Category = "Máy chiếu", Unit = "bộ", DepartmentId = d.Id, Total = 30, Damaged = 1 }, assetId);
            var laptopId = prefix + "-laptop";
            await Add(new AssetGroup { Id = laptopId, Code = "MAU-" + d.Code + "-MT", Name = "[MẪU] Máy tính xách tay", Category = "Máy tính", Unit = "chiếc", DepartmentId = d.Id, Total = 15 }, laptopId);
            foreach (var id in new[] { assetId, laptopId })
            {
                await Add(new EquipmentMovement { Id = id + "-stock", AssetGroupId = id, Kind = "stock.received", Quantity = id == assetId ? 30 : 15, ActorId = actor.Id, Note = "[MẪU] Số dư nhập kho ban đầu" }, id + "-stock");
                await Add(new FixedAssetProfile { AssetGroupId = id, AcquisitionValue = id == assetId ? 300_000_000 : 225_000_000, AcquisitionDate = today.AddMonths(-6), Source = "[MẪU] Ngân sách thiết bị", Reference = "MAU-HD-" + d.Code, Note = "Dữ liệu minh họa, không phải hóa đơn thực tế", ActorId = actor.Id }, id);
            }
            await db.SaveChangesAsync(ct);
            var inventoryId = prefix + "-inventory";
            await Add(new InventorySession { Id = inventoryId, Name = "[MẪU] Kiểm kê thiết bị " + d.Name, DepartmentId = d.Id }, inventoryId);
            await db.SaveChangesAsync(ct);
            foreach (var id in new[] { assetId, laptopId })
            {
                var a = await db.AssetGroups.SingleAsync(a => a.Id == id, ct);
                await Add(new InventoryItem { SessionId = inventoryId, AssetGroupId = id, Code = a.Code, Name = a.Name, Expected = a.Total, ExpectedDamaged = a.Damaged, ExpectedLost = a.Lost, AssetVersion = Convert.ToBase64String(a.RowVersion), Note = "[MẪU] Chờ kiểm đếm thực tế" }, inventoryId, id);
            }
            var maintenanceId = prefix + "-maintenance";
            await Add(new MaintenanceOrder { Id = maintenanceId, AssetGroupId = laptopId, DepartmentId = d.Id, Quantity = 1, Start = At(1, 8), End = At(2, 17), Content = "[MẪU] Vệ sinh và kiểm tra máy tính", Status = "planned" }, maintenanceId);
            var completedId = prefix + "-maintenance-completed";
            await Add(new MaintenanceOrder { Id = completedId, AssetGroupId = laptopId, DepartmentId = d.Id, Quantity = 1, Start = At(-7, 8), End = At(-6, 17), Content = "[MẪU] Bảo dưỡng định kỳ đã hoàn tất", Status = "completed", Result = "good", Cost = 500_000, Note = "[MẪU] Biên bản bảo dưỡng" }, completedId);
            await Add(new AssetDisposal { Id = prefix + "-disposal", AssetGroupId = assetId, DepartmentId = d.Id, Damaged = 1, Reason = "[MẪU] Thiết bị hỏng chờ đánh giá thanh lý", Reference = "MAU-TL-" + d.Code, ActorId = actor.Id }, prefix + "-disposal");
            var year = today.Year;
            // Finance sample records are explicitly labelled; they do affect the shared reports.
            foreach (var category in FinanceService.Categories)
            {
                var proposalId = prefix + "-budget-" + year + "-" + category;
                await Add(new BudgetProposal { Id = proposalId, DepartmentId = d.Id, Year = year, Category = category, Title = "[MẪU] Ngân sách " + category + " " + d.Name, Purpose = "Dự toán minh họa phục vụ trải nghiệm hệ thống", OwnerId = actor.Id, Status = "approved", ReviewerId = actor.Id, ReviewNote = "[MẪU] Phân bổ minh họa", Amount = 100_000_000, LinesJson = JsonSerializer.Serialize(new[] { new { Name = "[MẪU] Khoản dự toán", Quantity = 1, Price = 100_000_000 } }) }, proposalId);
                await db.SaveChangesAsync(ct);
                await Add(new BudgetAllocation { ProposalId = proposalId, Amount = 100_000_000 }, proposalId);
            }
            await Add(new BudgetProposal { Id = prefix + "-proposal-pending-" + year, DepartmentId = d.Id, Year = year, Category = "equipment", Title = "[MẪU] Dự toán mua sắm chờ duyệt", Purpose = "Minh họa luồng xét duyệt", OwnerId = actor.Id, Status = "pending", Amount = 20_000_000, LinesJson = JsonSerializer.Serialize(new[] { new { Name = "[MẪU] Thiết bị giảng dạy", Quantity = 2, Price = 10_000_000 } }) }, prefix + "-proposal-pending-" + year);
            await Add(new BudgetAdjustment { Id = prefix + "-adjustment-" + year, DepartmentId = d.Id, Year = year, Category = "equipment", Amount = 5_000_000, Reason = "[MẪU] Đề nghị bổ sung ngân sách thiết bị", ActorId = actor.Id }, prefix + "-adjustment-" + year);
            await Add(new FiscalPeriod { DepartmentId = d.Id, Year = year, ActorId = actor.Id, Reason = "[MẪU] Kỳ kế toán mở" }, d.Id, year);
            var paymentId = prefix + "-payment-" + year;
            await Add(new PaymentRequest { Id = paymentId, DepartmentId = d.Id, Year = year, Amount = 500_000, MaintenanceId = completedId, Content = "[MẪU] Thanh toán bảo dưỡng thiết bị", Status = "part_paid" }, paymentId);
            await db.SaveChangesAsync(ct);
            await Add(new FinanceEntry { Id = prefix + "-expense-" + year, DepartmentId = d.Id, Year = year, Category = "maintenance", Kind = "expense", Amount = 200_000, Voucher = "MAU-PC-" + d.Code + "-" + year, Date = today, Content = "[MẪU] Chi đợt 1 bảo dưỡng", ActorId = actor.Id, PaymentRequestId = paymentId }, prefix + "-expense-" + year);
            await Add(new FinanceEntry { Id = prefix + "-receipt-" + year, DepartmentId = d.Id, Year = year, Category = "general", Kind = "receipt", Amount = 2_000_000, Voucher = "MAU-PT-" + d.Code + "-" + year, Date = today, Content = "[MẪU] Khoản thu minh họa", ActorId = actor.Id }, prefix + "-receipt-" + year);
            await db.SaveChangesAsync(ct);
        }
        // Seed personal flows for existing lecturers without changing their identity or permissions.
        var lecturerIds = await db.UserRoles.Where(r => r.RoleId == "lecturer").Select(r => r.UserId).ToListAsync(ct);
        foreach (var u in users.Where(u => lecturerIds.Contains(u.Id) && departments.Any(d => d.Id == u.DepartmentId)))
        {
            var prefix = "sample-v1-user-" + u.Id;
            var roomId = prefix + "-room";
            var d = departments.Single(d => d.Id == u.DepartmentId);
            await Add(new Room { Id = roomId, Code = "MAU-P-" + u.Id.Replace("-", "")[..12], Name = "[MẪU] Phòng thực hành " + d.Name, Type = "Phòng máy", DepartmentId = d.Id, Capacity = 40 }, roomId);
            await db.SaveChangesAsync(ct);
            foreach (var (suffix, offset, status) in new[] { ("pending", 1, "pending"), ("past", -1, "approved"), ("today", 0, "approved") })
            {
                var requestId = prefix + "-" + suffix;
                await Add(new RoomRequest { Id = requestId, RoomId = roomId, OwnerId = u.Id, DepartmentId = d.Id, Start = At(offset, 9), End = At(offset, 11), Attendees = 25, Purpose = "[MẪU] Đăng ký phòng thực hành", Status = status, ReviewedBy = status == "approved" ? actor.Id : null, ReviewNote = status == "approved" ? "[MẪU] Lịch đã duyệt" : "" }, requestId);
                await db.SaveChangesAsync(ct);
                await Add(new RoomReservation { Id = requestId + "-slot", RoomId = roomId, RequestId = requestId, Start = At(offset, 9), End = At(offset, 11), Note = "[MẪU] Lịch minh họa" }, requestId + "-slot");
                if (suffix == "past") await Add(new RoomUseLog { Id = prefix + "-room-log", RequestId = requestId, Used = true, ActualStart = At(-1, 9), ActualEnd = At(-1, 11), Attendees = 25, Condition = "[MẪU] Phòng sạch, thiết bị hoạt động tốt", Issue = "[MẪU] Không có sự cố", Status = "submitted" }, prefix + "-room-log");
            }
            var assetId = "sample-v1-" + d.Id + "-projector";
            var loanId = prefix + "-loan";
            await Add(new EquipmentLoan { Id = loanId, AssetGroupId = assetId, OwnerId = u.Id, DepartmentId = d.Id, Start = At(-1, 8), End = At(-1, 17), Quantity = 2, Status = "return_pending", HandedAt = At(-1, 8), ReportedReceived = 2, Purpose = "[MẪU] Mượn thiết bị phục vụ giảng dạy", Note = "[MẪU] Chờ tiếp nhận trả" }, loanId);
            await db.SaveChangesAsync(ct);
            await Add(new EquipmentMovement { Id = loanId + "-handover", AssetGroupId = assetId, LoanId = loanId, Kind = "loan.handover", Quantity = 2, ActorId = actor.Id, Note = "[MẪU] Bàn giao minh họa" }, loanId + "-handover");
            await Add(new EquipmentReturn { Id = loanId + "-return", LoanId = loanId, Good = 2, Note = "[MẪU] Đề nghị trả đủ thiết bị" }, loanId + "-return");
            await Add(new EquipmentUseLog { Id = loanId + "-log", LoanId = loanId, Used = true, ActualStart = At(-1, 9), ActualEnd = At(-1, 11), Quantity = 2, Condition = "[MẪU] Thiết bị hoạt động tốt", Issue = "[MẪU] Không có sự cố", Status = "submitted" }, loanId + "-log");
            await db.SaveChangesAsync(ct);
        }
        if (added.Count > 0)
        {
            db.AuditEvents.Add(new AuditEvent { ActorId = actor.Id, Action = "sample-data.add", TargetId = "sample-v1", DetailJson = JsonSerializer.Serialize(new { source = "explicit-maintenance-command", marker = "[MẪU]", added }) });
            await db.SaveChangesAsync(ct);
        }
        await tx.CommitAsync(ct);
        return new { marker = "[MẪU]", added, note = "Không cập nhật hoặc xóa dữ liệu hiện có. Báo cáo tổng hợp bao gồm dữ liệu mẫu." };
    }
}
