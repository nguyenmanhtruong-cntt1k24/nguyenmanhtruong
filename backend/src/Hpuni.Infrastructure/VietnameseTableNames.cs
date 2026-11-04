using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class VietnameseTableNames
{
    public const string MigrationHistory = "LichSuCapNhatCauTruc";

    public static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
    {
        ["AspNetUsers"] = "NguoiDung",
        ["AspNetRoles"] = "VaiTro",
        ["AspNetUserRoles"] = "VaiTroNguoiDung",
        ["AspNetUserClaims"] = "ThuocTinhNguoiDung",
        ["AspNetRoleClaims"] = "ThuocTinhVaiTro",
        ["AspNetUserLogins"] = "DangNhapNgoai",
        ["AspNetUserTokens"] = "MaXacThucNguoiDung",
        ["Departments"] = "DonVi",
        ["RolePermissions"] = "QuyenVaiTro",
        ["RoleAccountOverrides"] = "TuyChinhVaiTroNguoiDung",
        ["UserDepartmentScopes"] = "PhamViDonViNguoiDung",
        ["UserPermissionDenials"] = "QuyenBiTuChoiNguoiDung",
        ["LoginSessions"] = "PhienDangNhap",
        ["AuditEvents"] = "NhatKyHeThong",
        ["Rooms"] = "Phong",
        ["RoomRequests"] = "PhieuDangKyPhong",
        ["RoomReservations"] = "LichDatPhong",
        ["RoomUseLogs"] = "NhatKySuDungPhong",
        ["IdempotencyRecords"] = "YeuCauChongLap",
        ["AssetGroups"] = "NhomTaiSan",
        ["EquipmentLoans"] = "PhieuMuonThietBi",
        ["EquipmentReturns"] = "PhieuTraThietBi",
        ["EquipmentMovements"] = "BienDongThietBi",
        ["EquipmentUseLogs"] = "NhatKySuDungThietBi",
        ["MaintenanceOrders"] = "PhieuBaoTri",
        ["InventorySessions"] = "DotKiemKe",
        ["InventoryItems"] = "ChiTietKiemKe",
        ["FixedAssetProfiles"] = "HoSoTaiSanCoDinh",
        ["AssetDisposals"] = "PhieuThanhLyTaiSan",
        ["FiscalPeriods"] = "KyKeToan",
        ["BudgetProposals"] = "DeXuatNganSach",
        ["BudgetAllocations"] = "PhanBoNganSach",
        ["BudgetAdjustments"] = "DieuChinhNganSach",
        ["PaymentRequests"] = "DeNghiThanhToan",
        ["FinanceEntries"] = "ChungTuTaiChinh",
        ["FinanceReversals"] = "ButToanDao",
        ["VoucherAttachments"] = "TepDinhKemChungTu",
        ["VoucherAttachmentData"] = "NoiDungTepChungTu",
        ["NotificationReadStates"] = "TrangThaiDocThongBao"
    };

    public static void Apply(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var oldName = entity.GetTableName();
            if (oldName is not null && Names.TryGetValue(oldName, out var newName))
                entity.SetTableName(newName);
        }
    }
}
