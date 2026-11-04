using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VietnameseTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_AspNetUsers_ActorId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_AspNetUsers_ReviewerId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_AssetGroups_AssetGroupId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetDisposals_Departments_DepartmentId",
                table: "AssetDisposals");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetGroups_Departments_DepartmentId",
                table: "AssetGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAdjustments_AspNetUsers_ActorId",
                table: "BudgetAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAdjustments_AspNetUsers_ReviewerId",
                table: "BudgetAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAdjustments_Departments_DepartmentId",
                table: "BudgetAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllocations_BudgetProposals_ProposalId",
                table: "BudgetAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAllocations_Departments_DepartmentId",
                table: "BudgetAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetProposals_AspNetUsers_OwnerId",
                table: "BudgetProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetProposals_AspNetUsers_ReviewerId",
                table: "BudgetProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetProposals_Departments_DepartmentId",
                table: "BudgetProposals");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentLoans_AspNetUsers_OwnerId",
                table: "EquipmentLoans");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentLoans_AssetGroups_AssetGroupId",
                table: "EquipmentLoans");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentLoans_Departments_DepartmentId",
                table: "EquipmentLoans");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMovements_AspNetUsers_ActorId",
                table: "EquipmentMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMovements_AssetGroups_AssetGroupId",
                table: "EquipmentMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentMovements_EquipmentLoans_LoanId",
                table: "EquipmentMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentReturns_AspNetUsers_AcceptedBy",
                table: "EquipmentReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentReturns_EquipmentLoans_LoanId",
                table: "EquipmentReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentUseLogs_EquipmentLoans_LoanId",
                table: "EquipmentUseLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_FinanceEntries_AspNetUsers_ActorId",
                table: "FinanceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FinanceEntries_Departments_DepartmentId",
                table: "FinanceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FinanceEntries_PaymentRequests_PaymentRequestId",
                table: "FinanceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FinanceReversals_AspNetUsers_ActorId",
                table: "FinanceReversals");

            migrationBuilder.DropForeignKey(
                name: "FK_FinanceReversals_FinanceEntries_EntryId",
                table: "FinanceReversals");

            migrationBuilder.DropForeignKey(
                name: "FK_FiscalPeriods_AspNetUsers_ActorId",
                table: "FiscalPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_FiscalPeriods_Departments_DepartmentId",
                table: "FiscalPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssetProfiles_AspNetUsers_ActorId",
                table: "FixedAssetProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedAssetProfiles_AssetGroups_AssetGroupId",
                table: "FixedAssetProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_AssetGroups_AssetGroupId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventorySessions_SessionId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventorySessions_Departments_DepartmentId",
                table: "InventorySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_LoginSessions_AspNetUsers_UserId",
                table: "LoginSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceOrders_AssetGroups_AssetGroupId",
                table: "MaintenanceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceOrders_Departments_DepartmentId",
                table: "MaintenanceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationReadStates_AspNetUsers_ActorId",
                table: "NotificationReadStates");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_Departments_DepartmentId",
                table: "PaymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_MaintenanceOrders_MaintenanceId",
                table: "PaymentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleAccountOverrides_AspNetUserRoles_UserId_RoleId",
                table: "RoleAccountOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRequests_AspNetUsers_OwnerId",
                table: "RoomRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRequests_Departments_DepartmentId",
                table: "RoomRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRequests_Rooms_RoomId",
                table: "RoomRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_RoomRequests_RequestId",
                table: "RoomReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_Rooms_RoomId",
                table: "RoomReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Departments_DepartmentId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUseLogs_RoomRequests_RequestId",
                table: "RoomUseLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartmentScopes_AspNetUsers_UserId",
                table: "UserDepartmentScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDepartmentScopes_Departments_DepartmentId",
                table: "UserDepartmentScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissionDenials_AspNetUsers_UserId",
                table: "UserPermissionDenials");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachmentData_VoucherAttachments_AttachmentId",
                table: "VoucherAttachmentData");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_AspNetUsers_ActorId",
                table: "VoucherAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_FinanceEntries_EntryId",
                table: "VoucherAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherAttachments",
                table: "VoucherAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherAttachmentData",
                table: "VoucherAttachmentData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermissionDenials",
                table: "UserPermissionDenials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDepartmentScopes",
                table: "UserDepartmentScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomUseLogs",
                table: "RoomUseLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomReservations",
                table: "RoomReservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomRequests",
                table: "RoomRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleAccountOverrides",
                table: "RoleAccountOverrides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentRequests",
                table: "PaymentRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationReadStates",
                table: "NotificationReadStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceOrders",
                table: "MaintenanceOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginSessions",
                table: "LoginSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventorySessions",
                table: "InventorySessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdempotencyRecords",
                table: "IdempotencyRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FixedAssetProfiles",
                table: "FixedAssetProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FiscalPeriods",
                table: "FiscalPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinanceReversals",
                table: "FinanceReversals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinanceEntries",
                table: "FinanceEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentUseLogs",
                table: "EquipmentUseLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentReturns",
                table: "EquipmentReturns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentMovements",
                table: "EquipmentMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipmentLoans",
                table: "EquipmentLoans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetProposals",
                table: "BudgetProposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetAllocations",
                table: "BudgetAllocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetAdjustments",
                table: "BudgetAdjustments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditEvents",
                table: "AuditEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetDisposals",
                table: "AssetDisposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "VoucherAttachments",
                newName: "TepDinhKemChungTu");

            migrationBuilder.RenameTable(
                name: "VoucherAttachmentData",
                newName: "NoiDungTepChungTu");

            migrationBuilder.RenameTable(
                name: "UserPermissionDenials",
                newName: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.RenameTable(
                name: "UserDepartmentScopes",
                newName: "PhamViDonViNguoiDung");

            migrationBuilder.RenameTable(
                name: "RoomUseLogs",
                newName: "NhatKySuDungPhong");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Phong");

            migrationBuilder.RenameTable(
                name: "RoomReservations",
                newName: "LichDatPhong");

            migrationBuilder.RenameTable(
                name: "RoomRequests",
                newName: "PhieuDangKyPhong");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "QuyenVaiTro");

            migrationBuilder.RenameTable(
                name: "RoleAccountOverrides",
                newName: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.RenameTable(
                name: "PaymentRequests",
                newName: "DeNghiThanhToan");

            migrationBuilder.RenameTable(
                name: "NotificationReadStates",
                newName: "TrangThaiDocThongBao");

            migrationBuilder.RenameTable(
                name: "MaintenanceOrders",
                newName: "PhieuBaoTri");

            migrationBuilder.RenameTable(
                name: "LoginSessions",
                newName: "PhienDangNhap");

            migrationBuilder.RenameTable(
                name: "InventorySessions",
                newName: "DotKiemKe");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "ChiTietKiemKe");

            migrationBuilder.RenameTable(
                name: "IdempotencyRecords",
                newName: "YeuCauChongLap");

            migrationBuilder.RenameTable(
                name: "FixedAssetProfiles",
                newName: "HoSoTaiSanCoDinh");

            migrationBuilder.RenameTable(
                name: "FiscalPeriods",
                newName: "KyKeToan");

            migrationBuilder.RenameTable(
                name: "FinanceReversals",
                newName: "ButToanDao");

            migrationBuilder.RenameTable(
                name: "FinanceEntries",
                newName: "ChungTuTaiChinh");

            migrationBuilder.RenameTable(
                name: "EquipmentUseLogs",
                newName: "NhatKySuDungThietBi");

            migrationBuilder.RenameTable(
                name: "EquipmentReturns",
                newName: "PhieuTraThietBi");

            migrationBuilder.RenameTable(
                name: "EquipmentMovements",
                newName: "BienDongThietBi");

            migrationBuilder.RenameTable(
                name: "EquipmentLoans",
                newName: "PhieuMuonThietBi");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "DonVi");

            migrationBuilder.RenameTable(
                name: "BudgetProposals",
                newName: "DeXuatNganSach");

            migrationBuilder.RenameTable(
                name: "BudgetAllocations",
                newName: "PhanBoNganSach");

            migrationBuilder.RenameTable(
                name: "BudgetAdjustments",
                newName: "DieuChinhNganSach");

            migrationBuilder.RenameTable(
                name: "AuditEvents",
                newName: "NhatKyHeThong");

            migrationBuilder.RenameTable(
                name: "AssetGroups",
                newName: "NhomTaiSan");

            migrationBuilder.RenameTable(
                name: "AssetDisposals",
                newName: "PhieuThanhLyTaiSan");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "MaXacThucNguoiDung");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "NguoiDung");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "VaiTroNguoiDung");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "DangNhapNgoai");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "ThuocTinhNguoiDung");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "VaiTro");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "ThuocTinhVaiTro");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherAttachments_EntryId",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_EntryId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherAttachments_ActorId",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDepartmentScopes_DepartmentId",
                table: "PhamViDonViNguoiDung",
                newName: "IX_PhamViDonViNguoiDung_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomUseLogs_RequestId",
                table: "NhatKySuDungPhong",
                newName: "IX_NhatKySuDungPhong_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_DepartmentId",
                table: "Phong",
                newName: "IX_Phong_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_Code",
                table: "Phong",
                newName: "IX_Phong_Code");

            migrationBuilder.RenameIndex(
                name: "IX_RoomReservations_RoomId_Active_Start_End",
                table: "LichDatPhong",
                newName: "IX_LichDatPhong_RoomId_Active_Start_End");

            migrationBuilder.RenameIndex(
                name: "IX_RoomReservations_RequestId",
                table: "LichDatPhong",
                newName: "IX_LichDatPhong_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRequests_RoomId",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRequests_OwnerId_CreatedAt",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRequests_DepartmentId",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequests_MaintenanceId",
                table: "DeNghiThanhToan",
                newName: "IX_DeNghiThanhToan_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentRequests_DepartmentId",
                table: "DeNghiThanhToan",
                newName: "IX_DeNghiThanhToan_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceOrders_DepartmentId",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceOrders_AssetGroupId_Status",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_LoginSessions_UserId",
                table: "PhienDangNhap",
                newName: "IX_PhienDangNhap_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InventorySessions_DepartmentId",
                table: "DotKiemKe",
                newName: "IX_DotKiemKe_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_AssetGroupId",
                table: "ChiTietKiemKe",
                newName: "IX_ChiTietKiemKe_AssetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_FixedAssetProfiles_ActorId",
                table: "HoSoTaiSanCoDinh",
                newName: "IX_HoSoTaiSanCoDinh_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_FiscalPeriods_ActorId",
                table: "KyKeToan",
                newName: "IX_KyKeToan_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceReversals_EntryId",
                table: "ButToanDao",
                newName: "IX_ButToanDao_EntryId");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceReversals_ActorId",
                table: "ButToanDao",
                newName: "IX_ButToanDao_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceEntries_Year_NormalizedVoucher",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_Year_NormalizedVoucher");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_PaymentRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceEntries_DepartmentId_Year_Category_Kind",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_DepartmentId_Year_Category_Kind");

            migrationBuilder.RenameIndex(
                name: "IX_FinanceEntries_ActorId",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentUseLogs_LoanId",
                table: "NhatKySuDungThietBi",
                newName: "IX_NhatKySuDungThietBi_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentReturns_LoanId",
                table: "PhieuTraThietBi",
                newName: "IX_PhieuTraThietBi_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentReturns_AcceptedBy",
                table: "PhieuTraThietBi",
                newName: "IX_PhieuTraThietBi_AcceptedBy");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentMovements_LoanId",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentMovements_AssetGroupId_At",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_AssetGroupId_At");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentMovements_ActorId",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentLoans_OwnerId_CreatedAt",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentLoans_DepartmentId",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentLoans_AssetGroupId_Status_Start_End",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_AssetGroupId_Status_Start_End");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_NormalizedName",
                table: "DonVi",
                newName: "IX_DonVi_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_Code",
                table: "DonVi",
                newName: "IX_DonVi_Code");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetProposals_ReviewerId",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetProposals_OwnerId",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetProposals_DepartmentId",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAllocations_ProposalId",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAllocations_DepartmentId_Year_Category",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_DepartmentId_Year_Category");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAdjustments_ReviewerId",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAdjustments_DepartmentId_Year_Category_Status",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_DepartmentId_Year_Category_Status");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAdjustments_ActorId",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditEvents_At",
                table: "NhatKyHeThong",
                newName: "IX_NhatKyHeThong_At");

            migrationBuilder.RenameIndex(
                name: "IX_AssetGroups_DepartmentId",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetGroups_Code",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_Code");

            migrationBuilder.RenameIndex(
                name: "IX_AssetDisposals_ReviewerId",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetDisposals_DepartmentId_Status_CreatedAt",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_DepartmentId_Status_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AssetDisposals_AssetGroupId_Status",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_AssetDisposals_ActorId",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "NguoiDung",
                newName: "IX_NguoiDung_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "VaiTroNguoiDung",
                newName: "IX_VaiTroNguoiDung_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "DangNhapNgoai",
                newName: "IX_DangNhapNgoai_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "ThuocTinhNguoiDung",
                newName: "IX_ThuocTinhNguoiDung_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "ThuocTinhVaiTro",
                newName: "IX_ThuocTinhVaiTro_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TepDinhKemChungTu",
                table: "TepDinhKemChungTu",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoiDungTepChungTu",
                table: "NoiDungTepChungTu",
                column: "AttachmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuyenBiTuChoiNguoiDung",
                table: "QuyenBiTuChoiNguoiDung",
                columns: new[] { "UserId", "Permission" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhamViDonViNguoiDung",
                table: "PhamViDonViNguoiDung",
                columns: new[] { "UserId", "DepartmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhatKySuDungPhong",
                table: "NhatKySuDungPhong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Phong",
                table: "Phong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LichDatPhong",
                table: "LichDatPhong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuDangKyPhong",
                table: "PhieuDangKyPhong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuyenVaiTro",
                table: "QuyenVaiTro",
                columns: new[] { "RoleId", "Permission" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TuyChinhVaiTroNguoiDung",
                table: "TuyChinhVaiTroNguoiDung",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeNghiThanhToan",
                table: "DeNghiThanhToan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrangThaiDocThongBao",
                table: "TrangThaiDocThongBao",
                columns: new[] { "ActorId", "SourceKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuBaoTri",
                table: "PhieuBaoTri",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhienDangNhap",
                table: "PhienDangNhap",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DotKiemKe",
                table: "DotKiemKe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietKiemKe",
                table: "ChiTietKiemKe",
                columns: new[] { "SessionId", "AssetGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_YeuCauChongLap",
                table: "YeuCauChongLap",
                columns: new[] { "ActorId", "Key" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoSoTaiSanCoDinh",
                table: "HoSoTaiSanCoDinh",
                column: "AssetGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KyKeToan",
                table: "KyKeToan",
                columns: new[] { "DepartmentId", "Year" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ButToanDao",
                table: "ButToanDao",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChungTuTaiChinh",
                table: "ChungTuTaiChinh",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhatKySuDungThietBi",
                table: "NhatKySuDungThietBi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuTraThietBi",
                table: "PhieuTraThietBi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BienDongThietBi",
                table: "BienDongThietBi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuMuonThietBi",
                table: "PhieuMuonThietBi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonVi",
                table: "DonVi",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeXuatNganSach",
                table: "DeXuatNganSach",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanBoNganSach",
                table: "PhanBoNganSach",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DieuChinhNganSach",
                table: "DieuChinhNganSach",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhatKyHeThong",
                table: "NhatKyHeThong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhomTaiSan",
                table: "NhomTaiSan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhieuThanhLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaXacThucNguoiDung",
                table: "MaXacThucNguoiDung",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaiTroNguoiDung",
                table: "VaiTroNguoiDung",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DangNhapNgoai",
                table: "DangNhapNgoai",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThuocTinhNguoiDung",
                table: "ThuocTinhNguoiDung",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaiTro",
                table: "VaiTro",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThuocTinhVaiTro",
                table: "ThuocTinhVaiTro",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_ActorId",
                table: "BienDongThietBi",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_AssetGroupId",
                table: "BienDongThietBi",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_LoanId",
                table: "BienDongThietBi",
                column: "LoanId",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_EntryId",
                table: "ButToanDao",
                column: "EntryId",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_NguoiDung_ActorId",
                table: "ButToanDao",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_SessionId",
                table: "ChiTietKiemKe",
                column: "SessionId",
                principalTable: "DotKiemKe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_AssetGroupId",
                table: "ChiTietKiemKe",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_PaymentRequestId",
                table: "ChungTuTaiChinh",
                column: "PaymentRequestId",
                principalTable: "DeNghiThanhToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_DepartmentId",
                table: "ChungTuTaiChinh",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_ActorId",
                table: "ChungTuTaiChinh",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_UserId",
                table: "DangNhapNgoai",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_DepartmentId",
                table: "DeNghiThanhToan",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaintenanceId",
                table: "DeNghiThanhToan",
                column: "MaintenanceId",
                principalTable: "PhieuBaoTri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_DonVi_DepartmentId",
                table: "DeXuatNganSach",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_OwnerId",
                table: "DeXuatNganSach",
                column: "OwnerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_ReviewerId",
                table: "DeXuatNganSach",
                column: "ReviewerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_DepartmentId",
                table: "DieuChinhNganSach",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ActorId",
                table: "DieuChinhNganSach",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ReviewerId",
                table: "DieuChinhNganSach",
                column: "ReviewerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKiemKe_DonVi_DepartmentId",
                table: "DotKiemKe",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_ActorId",
                table: "HoSoTaiSanCoDinh",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_AssetGroupId",
                table: "HoSoTaiSanCoDinh",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_DonVi_DepartmentId",
                table: "KyKeToan",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_NguoiDung_ActorId",
                table: "KyKeToan",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_RequestId",
                table: "LichDatPhong",
                column: "RequestId",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_Phong_RoomId",
                table: "LichDatPhong",
                column: "RoomId",
                principalTable: "Phong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_UserId",
                table: "MaXacThucNguoiDung",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_DonVi_DepartmentId",
                table: "NguoiDung",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_RequestId",
                table: "NhatKySuDungPhong",
                column: "RequestId",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_LoanId",
                table: "NhatKySuDungThietBi",
                column: "LoanId",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhomTaiSan_DonVi_DepartmentId",
                table: "NhomTaiSan",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_AttachmentId",
                table: "NoiDungTepChungTu",
                column: "AttachmentId",
                principalTable: "TepDinhKemChungTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_DepartmentId",
                table: "PhamViDonViNguoiDung",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_UserId",
                table: "PhamViDonViNguoiDung",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_ProposalId",
                table: "PhanBoNganSach",
                column: "ProposalId",
                principalTable: "DeXuatNganSach",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DonVi_DepartmentId",
                table: "PhanBoNganSach",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_UserId",
                table: "PhienDangNhap",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_DonVi_DepartmentId",
                table: "PhieuBaoTri",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_AssetGroupId",
                table: "PhieuBaoTri",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_DepartmentId",
                table: "PhieuDangKyPhong",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_OwnerId",
                table: "PhieuDangKyPhong",
                column: "OwnerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_RoomId",
                table: "PhieuDangKyPhong",
                column: "RoomId",
                principalTable: "Phong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_DepartmentId",
                table: "PhieuMuonThietBi",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_OwnerId",
                table: "PhieuMuonThietBi",
                column: "OwnerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_AssetGroupId",
                table: "PhieuMuonThietBi",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_DepartmentId",
                table: "PhieuThanhLyTaiSan",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ActorId",
                table: "PhieuThanhLyTaiSan",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ReviewerId",
                table: "PhieuThanhLyTaiSan",
                column: "ReviewerId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_AssetGroupId",
                table: "PhieuThanhLyTaiSan",
                column: "AssetGroupId",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_AcceptedBy",
                table: "PhieuTraThietBi",
                column: "AcceptedBy",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_LoanId",
                table: "PhieuTraThietBi",
                column: "LoanId",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phong_DonVi_DepartmentId",
                table: "Phong",
                column: "DepartmentId",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_UserId",
                table: "QuyenBiTuChoiNguoiDung",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_RoleId",
                table: "QuyenVaiTro",
                column: "RoleId",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_EntryId",
                table: "TepDinhKemChungTu",
                column: "EntryId",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_ActorId",
                table: "TepDinhKemChungTu",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_UserId",
                table: "ThuocTinhNguoiDung",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_RoleId",
                table: "ThuocTinhVaiTro",
                column: "RoleId",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_ActorId",
                table: "TrangThaiDocThongBao",
                column: "ActorId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_UserId_RoleId",
                table: "TuyChinhVaiTroNguoiDung",
                columns: new[] { "UserId", "RoleId" },
                principalTable: "VaiTroNguoiDung",
                principalColumns: new[] { "UserId", "RoleId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_UserId",
                table: "VaiTroNguoiDung",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_RoleId",
                table: "VaiTroNguoiDung",
                column: "RoleId",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_ActorId",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_AssetGroupId",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_LoanId",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_EntryId",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_NguoiDung_ActorId",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_SessionId",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_AssetGroupId",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_PaymentRequestId",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_DepartmentId",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_ActorId",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_UserId",
                table: "DangNhapNgoai");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_DepartmentId",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaintenanceId",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_DonVi_DepartmentId",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_OwnerId",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_ReviewerId",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_DepartmentId",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ActorId",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ReviewerId",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKiemKe_DonVi_DepartmentId",
                table: "DotKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_ActorId",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_AssetGroupId",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_DonVi_DepartmentId",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_NguoiDung_ActorId",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_RequestId",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_Phong_RoomId",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_UserId",
                table: "MaXacThucNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_DonVi_DepartmentId",
                table: "NguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_RequestId",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_LoanId",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_NhomTaiSan_DonVi_DepartmentId",
                table: "NhomTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_AttachmentId",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_DepartmentId",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_UserId",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_ProposalId",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DonVi_DepartmentId",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_UserId",
                table: "PhienDangNhap");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_DonVi_DepartmentId",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_AssetGroupId",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_DepartmentId",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_OwnerId",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_RoomId",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_DepartmentId",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_OwnerId",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_AssetGroupId",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_DepartmentId",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ActorId",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ReviewerId",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_AssetGroupId",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_AcceptedBy",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_LoanId",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_Phong_DonVi_DepartmentId",
                table: "Phong");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_UserId",
                table: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_RoleId",
                table: "QuyenVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_EntryId",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_ActorId",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_UserId",
                table: "ThuocTinhNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_RoleId",
                table: "ThuocTinhVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_ActorId",
                table: "TrangThaiDocThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_UserId_RoleId",
                table: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_UserId",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_RoleId",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YeuCauChongLap",
                table: "YeuCauChongLap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaiTroNguoiDung",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaiTro",
                table: "VaiTro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TuyChinhVaiTroNguoiDung",
                table: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrangThaiDocThongBao",
                table: "TrangThaiDocThongBao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThuocTinhVaiTro",
                table: "ThuocTinhVaiTro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThuocTinhNguoiDung",
                table: "ThuocTinhNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TepDinhKemChungTu",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuyenVaiTro",
                table: "QuyenVaiTro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuyenBiTuChoiNguoiDung",
                table: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Phong",
                table: "Phong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuTraThietBi",
                table: "PhieuTraThietBi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuThanhLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuMuonThietBi",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuDangKyPhong",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhieuBaoTri",
                table: "PhieuBaoTri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhienDangNhap",
                table: "PhienDangNhap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanBoNganSach",
                table: "PhanBoNganSach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhamViDonViNguoiDung",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoiDungTepChungTu",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhomTaiSan",
                table: "NhomTaiSan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhatKySuDungThietBi",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhatKySuDungPhong",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhatKyHeThong",
                table: "NhatKyHeThong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaXacThucNguoiDung",
                table: "MaXacThucNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LichDatPhong",
                table: "LichDatPhong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KyKeToan",
                table: "KyKeToan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoSoTaiSanCoDinh",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DotKiemKe",
                table: "DotKiemKe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonVi",
                table: "DonVi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DieuChinhNganSach",
                table: "DieuChinhNganSach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeXuatNganSach",
                table: "DeXuatNganSach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeNghiThanhToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DangNhapNgoai",
                table: "DangNhapNgoai");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChungTuTaiChinh",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietKiemKe",
                table: "ChiTietKiemKe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ButToanDao",
                table: "ButToanDao");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BienDongThietBi",
                table: "BienDongThietBi");

            migrationBuilder.RenameTable(
                name: "YeuCauChongLap",
                newName: "IdempotencyRecords");

            migrationBuilder.RenameTable(
                name: "VaiTroNguoiDung",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "VaiTro",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "TuyChinhVaiTroNguoiDung",
                newName: "RoleAccountOverrides");

            migrationBuilder.RenameTable(
                name: "TrangThaiDocThongBao",
                newName: "NotificationReadStates");

            migrationBuilder.RenameTable(
                name: "ThuocTinhVaiTro",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "ThuocTinhNguoiDung",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "TepDinhKemChungTu",
                newName: "VoucherAttachments");

            migrationBuilder.RenameTable(
                name: "QuyenVaiTro",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "QuyenBiTuChoiNguoiDung",
                newName: "UserPermissionDenials");

            migrationBuilder.RenameTable(
                name: "Phong",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "PhieuTraThietBi",
                newName: "EquipmentReturns");

            migrationBuilder.RenameTable(
                name: "PhieuThanhLyTaiSan",
                newName: "AssetDisposals");

            migrationBuilder.RenameTable(
                name: "PhieuMuonThietBi",
                newName: "EquipmentLoans");

            migrationBuilder.RenameTable(
                name: "PhieuDangKyPhong",
                newName: "RoomRequests");

            migrationBuilder.RenameTable(
                name: "PhieuBaoTri",
                newName: "MaintenanceOrders");

            migrationBuilder.RenameTable(
                name: "PhienDangNhap",
                newName: "LoginSessions");

            migrationBuilder.RenameTable(
                name: "PhanBoNganSach",
                newName: "BudgetAllocations");

            migrationBuilder.RenameTable(
                name: "PhamViDonViNguoiDung",
                newName: "UserDepartmentScopes");

            migrationBuilder.RenameTable(
                name: "NoiDungTepChungTu",
                newName: "VoucherAttachmentData");

            migrationBuilder.RenameTable(
                name: "NhomTaiSan",
                newName: "AssetGroups");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungThietBi",
                newName: "EquipmentUseLogs");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungPhong",
                newName: "RoomUseLogs");

            migrationBuilder.RenameTable(
                name: "NhatKyHeThong",
                newName: "AuditEvents");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "MaXacThucNguoiDung",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "LichDatPhong",
                newName: "RoomReservations");

            migrationBuilder.RenameTable(
                name: "KyKeToan",
                newName: "FiscalPeriods");

            migrationBuilder.RenameTable(
                name: "HoSoTaiSanCoDinh",
                newName: "FixedAssetProfiles");

            migrationBuilder.RenameTable(
                name: "DotKiemKe",
                newName: "InventorySessions");

            migrationBuilder.RenameTable(
                name: "DonVi",
                newName: "Departments");

            migrationBuilder.RenameTable(
                name: "DieuChinhNganSach",
                newName: "BudgetAdjustments");

            migrationBuilder.RenameTable(
                name: "DeXuatNganSach",
                newName: "BudgetProposals");

            migrationBuilder.RenameTable(
                name: "DeNghiThanhToan",
                newName: "PaymentRequests");

            migrationBuilder.RenameTable(
                name: "DangNhapNgoai",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "ChungTuTaiChinh",
                newName: "FinanceEntries");

            migrationBuilder.RenameTable(
                name: "ChiTietKiemKe",
                newName: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "ButToanDao",
                newName: "FinanceReversals");

            migrationBuilder.RenameTable(
                name: "BienDongThietBi",
                newName: "EquipmentMovements");

            migrationBuilder.RenameIndex(
                name: "IX_VaiTroNguoiDung_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhVaiTro_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhNguoiDung_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_EntryId",
                table: "VoucherAttachments",
                newName: "IX_VoucherAttachments_EntryId");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_ActorId",
                table: "VoucherAttachments",
                newName: "IX_VoucherAttachments_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_DepartmentId",
                table: "Rooms",
                newName: "IX_Rooms_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_Code",
                table: "Rooms",
                newName: "IX_Rooms_Code");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuTraThietBi_LoanId",
                table: "EquipmentReturns",
                newName: "IX_EquipmentReturns_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuTraThietBi_AcceptedBy",
                table: "EquipmentReturns",
                newName: "IX_EquipmentReturns_AcceptedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_ReviewerId",
                table: "AssetDisposals",
                newName: "IX_AssetDisposals_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_DepartmentId_Status_CreatedAt",
                table: "AssetDisposals",
                newName: "IX_AssetDisposals_DepartmentId_Status_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_AssetGroupId_Status",
                table: "AssetDisposals",
                newName: "IX_AssetDisposals_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_ActorId",
                table: "AssetDisposals",
                newName: "IX_AssetDisposals_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_OwnerId_CreatedAt",
                table: "EquipmentLoans",
                newName: "IX_EquipmentLoans_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_DepartmentId",
                table: "EquipmentLoans",
                newName: "IX_EquipmentLoans_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_AssetGroupId_Status_Start_End",
                table: "EquipmentLoans",
                newName: "IX_EquipmentLoans_AssetGroupId_Status_Start_End");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_RoomId",
                table: "RoomRequests",
                newName: "IX_RoomRequests_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_OwnerId_CreatedAt",
                table: "RoomRequests",
                newName: "IX_RoomRequests_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_DepartmentId",
                table: "RoomRequests",
                newName: "IX_RoomRequests_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_DepartmentId",
                table: "MaintenanceOrders",
                newName: "IX_MaintenanceOrders_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_AssetGroupId_Status",
                table: "MaintenanceOrders",
                newName: "IX_MaintenanceOrders_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_PhienDangNhap_UserId",
                table: "LoginSessions",
                newName: "IX_LoginSessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_ProposalId",
                table: "BudgetAllocations",
                newName: "IX_BudgetAllocations_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_DepartmentId_Year_Category",
                table: "BudgetAllocations",
                newName: "IX_BudgetAllocations_DepartmentId_Year_Category");

            migrationBuilder.RenameIndex(
                name: "IX_PhamViDonViNguoiDung_DepartmentId",
                table: "UserDepartmentScopes",
                newName: "IX_UserDepartmentScopes_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_DepartmentId",
                table: "AssetGroups",
                newName: "IX_AssetGroups_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_Code",
                table: "AssetGroups",
                newName: "IX_AssetGroups_Code");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungThietBi_LoanId",
                table: "EquipmentUseLogs",
                newName: "IX_EquipmentUseLogs_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungPhong_RequestId",
                table: "RoomUseLogs",
                newName: "IX_RoomUseLogs_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKyHeThong_At",
                table: "AuditEvents",
                newName: "IX_AuditEvents_At");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_DepartmentId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_LichDatPhong_RoomId_Active_Start_End",
                table: "RoomReservations",
                newName: "IX_RoomReservations_RoomId_Active_Start_End");

            migrationBuilder.RenameIndex(
                name: "IX_LichDatPhong_RequestId",
                table: "RoomReservations",
                newName: "IX_RoomReservations_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_KyKeToan_ActorId",
                table: "FiscalPeriods",
                newName: "IX_FiscalPeriods_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_HoSoTaiSanCoDinh_ActorId",
                table: "FixedAssetProfiles",
                newName: "IX_FixedAssetProfiles_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_DotKiemKe_DepartmentId",
                table: "InventorySessions",
                newName: "IX_InventorySessions_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_NormalizedName",
                table: "Departments",
                newName: "IX_Departments_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_Code",
                table: "Departments",
                newName: "IX_Departments_Code");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_ReviewerId",
                table: "BudgetAdjustments",
                newName: "IX_BudgetAdjustments_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_DepartmentId_Year_Category_Status",
                table: "BudgetAdjustments",
                newName: "IX_BudgetAdjustments_DepartmentId_Year_Category_Status");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_ActorId",
                table: "BudgetAdjustments",
                newName: "IX_BudgetAdjustments_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_ReviewerId",
                table: "BudgetProposals",
                newName: "IX_BudgetProposals_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_OwnerId",
                table: "BudgetProposals",
                newName: "IX_BudgetProposals_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_DepartmentId",
                table: "BudgetProposals",
                newName: "IX_BudgetProposals_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DeNghiThanhToan_MaintenanceId",
                table: "PaymentRequests",
                newName: "IX_PaymentRequests_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_DeNghiThanhToan_DepartmentId",
                table: "PaymentRequests",
                newName: "IX_PaymentRequests_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DangNhapNgoai_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_Year_NormalizedVoucher",
                table: "FinanceEntries",
                newName: "IX_FinanceEntries_Year_NormalizedVoucher");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_PaymentRequestId",
                table: "FinanceEntries",
                newName: "IX_FinanceEntries_PaymentRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_DepartmentId_Year_Category_Kind",
                table: "FinanceEntries",
                newName: "IX_FinanceEntries_DepartmentId_Year_Category_Kind");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_ActorId",
                table: "FinanceEntries",
                newName: "IX_FinanceEntries_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietKiemKe_AssetGroupId",
                table: "InventoryItems",
                newName: "IX_InventoryItems_AssetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_EntryId",
                table: "FinanceReversals",
                newName: "IX_FinanceReversals_EntryId");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_ActorId",
                table: "FinanceReversals",
                newName: "IX_FinanceReversals_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_LoanId",
                table: "EquipmentMovements",
                newName: "IX_EquipmentMovements_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_AssetGroupId_At",
                table: "EquipmentMovements",
                newName: "IX_EquipmentMovements_AssetGroupId_At");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_ActorId",
                table: "EquipmentMovements",
                newName: "IX_EquipmentMovements_ActorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdempotencyRecords",
                table: "IdempotencyRecords",
                columns: new[] { "ActorId", "Key" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleAccountOverrides",
                table: "RoleAccountOverrides",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationReadStates",
                table: "NotificationReadStates",
                columns: new[] { "ActorId", "SourceKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherAttachments",
                table: "VoucherAttachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "Permission" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermissionDenials",
                table: "UserPermissionDenials",
                columns: new[] { "UserId", "Permission" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentReturns",
                table: "EquipmentReturns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetDisposals",
                table: "AssetDisposals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentLoans",
                table: "EquipmentLoans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomRequests",
                table: "RoomRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceOrders",
                table: "MaintenanceOrders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginSessions",
                table: "LoginSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetAllocations",
                table: "BudgetAllocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDepartmentScopes",
                table: "UserDepartmentScopes",
                columns: new[] { "UserId", "DepartmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherAttachmentData",
                table: "VoucherAttachmentData",
                column: "AttachmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetGroups",
                table: "AssetGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentUseLogs",
                table: "EquipmentUseLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomUseLogs",
                table: "RoomUseLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditEvents",
                table: "AuditEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomReservations",
                table: "RoomReservations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FiscalPeriods",
                table: "FiscalPeriods",
                columns: new[] { "DepartmentId", "Year" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FixedAssetProfiles",
                table: "FixedAssetProfiles",
                column: "AssetGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventorySessions",
                table: "InventorySessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetAdjustments",
                table: "BudgetAdjustments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetProposals",
                table: "BudgetProposals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentRequests",
                table: "PaymentRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinanceEntries",
                table: "FinanceEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                columns: new[] { "SessionId", "AssetGroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinanceReversals",
                table: "FinanceReversals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipmentMovements",
                table: "EquipmentMovements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_AspNetUsers_ActorId",
                table: "AssetDisposals",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_AspNetUsers_ReviewerId",
                table: "AssetDisposals",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_AssetGroups_AssetGroupId",
                table: "AssetDisposals",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDisposals_Departments_DepartmentId",
                table: "AssetDisposals",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetGroups_Departments_DepartmentId",
                table: "AssetGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAdjustments_AspNetUsers_ActorId",
                table: "BudgetAdjustments",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAdjustments_AspNetUsers_ReviewerId",
                table: "BudgetAdjustments",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAdjustments_Departments_DepartmentId",
                table: "BudgetAdjustments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAllocations_BudgetProposals_ProposalId",
                table: "BudgetAllocations",
                column: "ProposalId",
                principalTable: "BudgetProposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAllocations_Departments_DepartmentId",
                table: "BudgetAllocations",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetProposals_AspNetUsers_OwnerId",
                table: "BudgetProposals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetProposals_AspNetUsers_ReviewerId",
                table: "BudgetProposals",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetProposals_Departments_DepartmentId",
                table: "BudgetProposals",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentLoans_AspNetUsers_OwnerId",
                table: "EquipmentLoans",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentLoans_AssetGroups_AssetGroupId",
                table: "EquipmentLoans",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentLoans_Departments_DepartmentId",
                table: "EquipmentLoans",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMovements_AspNetUsers_ActorId",
                table: "EquipmentMovements",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMovements_AssetGroups_AssetGroupId",
                table: "EquipmentMovements",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentMovements_EquipmentLoans_LoanId",
                table: "EquipmentMovements",
                column: "LoanId",
                principalTable: "EquipmentLoans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentReturns_AspNetUsers_AcceptedBy",
                table: "EquipmentReturns",
                column: "AcceptedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentReturns_EquipmentLoans_LoanId",
                table: "EquipmentReturns",
                column: "LoanId",
                principalTable: "EquipmentLoans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentUseLogs_EquipmentLoans_LoanId",
                table: "EquipmentUseLogs",
                column: "LoanId",
                principalTable: "EquipmentLoans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinanceEntries_AspNetUsers_ActorId",
                table: "FinanceEntries",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinanceEntries_Departments_DepartmentId",
                table: "FinanceEntries",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinanceEntries_PaymentRequests_PaymentRequestId",
                table: "FinanceEntries",
                column: "PaymentRequestId",
                principalTable: "PaymentRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinanceReversals_AspNetUsers_ActorId",
                table: "FinanceReversals",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinanceReversals_FinanceEntries_EntryId",
                table: "FinanceReversals",
                column: "EntryId",
                principalTable: "FinanceEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FiscalPeriods_AspNetUsers_ActorId",
                table: "FiscalPeriods",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FiscalPeriods_Departments_DepartmentId",
                table: "FiscalPeriods",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssetProfiles_AspNetUsers_ActorId",
                table: "FixedAssetProfiles",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedAssetProfiles_AssetGroups_AssetGroupId",
                table: "FixedAssetProfiles",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_AssetGroups_AssetGroupId",
                table: "InventoryItems",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventorySessions_SessionId",
                table: "InventoryItems",
                column: "SessionId",
                principalTable: "InventorySessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventorySessions_Departments_DepartmentId",
                table: "InventorySessions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginSessions_AspNetUsers_UserId",
                table: "LoginSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceOrders_AssetGroups_AssetGroupId",
                table: "MaintenanceOrders",
                column: "AssetGroupId",
                principalTable: "AssetGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceOrders_Departments_DepartmentId",
                table: "MaintenanceOrders",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationReadStates_AspNetUsers_ActorId",
                table: "NotificationReadStates",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Departments_DepartmentId",
                table: "PaymentRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_MaintenanceOrders_MaintenanceId",
                table: "PaymentRequests",
                column: "MaintenanceId",
                principalTable: "MaintenanceOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleAccountOverrides_AspNetUserRoles_UserId_RoleId",
                table: "RoleAccountOverrides",
                columns: new[] { "UserId", "RoleId" },
                principalTable: "AspNetUserRoles",
                principalColumns: new[] { "UserId", "RoleId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRequests_AspNetUsers_OwnerId",
                table: "RoomRequests",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRequests_Departments_DepartmentId",
                table: "RoomRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRequests_Rooms_RoomId",
                table: "RoomRequests",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_RoomRequests_RequestId",
                table: "RoomReservations",
                column: "RequestId",
                principalTable: "RoomRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_Rooms_RoomId",
                table: "RoomReservations",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Departments_DepartmentId",
                table: "Rooms",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUseLogs_RoomRequests_RequestId",
                table: "RoomUseLogs",
                column: "RequestId",
                principalTable: "RoomRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartmentScopes_AspNetUsers_UserId",
                table: "UserDepartmentScopes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDepartmentScopes_Departments_DepartmentId",
                table: "UserDepartmentScopes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissionDenials_AspNetUsers_UserId",
                table: "UserPermissionDenials",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachmentData_VoucherAttachments_AttachmentId",
                table: "VoucherAttachmentData",
                column: "AttachmentId",
                principalTable: "VoucherAttachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_AspNetUsers_ActorId",
                table: "VoucherAttachments",
                column: "ActorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_FinanceEntries_EntryId",
                table: "VoucherAttachments",
                column: "EntryId",
                principalTable: "FinanceEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
