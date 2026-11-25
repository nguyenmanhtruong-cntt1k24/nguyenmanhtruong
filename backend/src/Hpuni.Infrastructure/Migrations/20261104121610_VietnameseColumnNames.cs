using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VietnameseColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_LoanId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_EntryId",
                schema: "KeToan",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_NguoiDung_ActorId",
                schema: "KeToan",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_SessionId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_PaymentRequestId",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_DepartmentId",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_ActorId",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "DangNhapNgoai");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_DepartmentId",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaintenanceId",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_OwnerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_ReviewerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ActorId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ReviewerId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKiemKe_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_DonVi_DepartmentId",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_NguoiDung_ActorId",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_RequestId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_Phong_RoomId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_DonVi_DepartmentId",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_RequestId",
                schema: "GiangVien",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_LoanId",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_NhomTaiSan_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_AttachmentId",
                schema: "KeToan",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_DepartmentId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_ProposalId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_DepartmentId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_OwnerId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_RoomId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_DepartmentId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_OwnerId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_AssetGroupId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ReviewerId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_AcceptedBy",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_LoanId",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_Phong_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "Phong");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "QuyenVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_EntryId",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_ActorId",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_ActorId",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_UserId_RoleId",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attachment_Size",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Room_Capacity",
                schema: "QuanLyTaiSan",
                table: "Phong");

            migrationBuilder.DropIndex(
                name: "IX_PhieuTraThietBi_LoanId",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Return_Quantity",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetDisposal_Quantity",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetDisposal_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Interval",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Quantity",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Status",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Attendees",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Interval",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Maintenance_Valid",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Allocation_Amount",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AttachmentData_Size",
                schema: "KeToan",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Asset_Quantity",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EquipmentLog_Valid",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RoomLog_Actual",
                schema: "GiangVien",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_LichDatPhong_RequestId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservation_Interval",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FiscalPeriod_Year",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FixedAssetProfile_Value",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_Status",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Adjustment_Amount",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Proposal_Amount",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropIndex(
                name: "IX_DeNghiThanhToan_MaintenanceId",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payment_Amount",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_Count",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movement_Quantity",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.RenameColumn(
                name: "ResultJson",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "KetQuaJson");

            migrationBuilder.RenameColumn(
                name: "PayloadHash",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "MaBamNoiDung");

            migrationBuilder.RenameColumn(
                name: "Operation",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "ThaoTac");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Key",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "Khoa");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameIndex(
                name: "IX_VaiTroNguoiDung_RoleId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "IX_VaiTroNguoiDung_MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "Scope",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "PhamVi");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "TenChuanHoa");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "DauDongThoi");

            migrationBuilder.RenameColumn(
                name: "BuiltIn",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "MacDinhHeThong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "Scope",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "PhamVi");

            migrationBuilder.RenameColumn(
                name: "PermissionsJson",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "DanhSachQuyenJson");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "SourceVersion",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "PhienBanNguon");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "NgayDoc");

            migrationBuilder.RenameColumn(
                name: "SourceKey",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "KhoaNguon");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "GiaTriThuocTinh");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "LoaiThuocTinh");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhVaiTro_RoleId",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "IX_ThuocTinhVaiTro_MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "GiaTriThuocTinh");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "LoaiThuocTinh");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhNguoiDung_UserId",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "IX_ThuocTinhNguoiDung_MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Size",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "KichThuoc");

            migrationBuilder.RenameColumn(
                name: "Sha256",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "MaBamSha256");

            migrationBuilder.RenameColumn(
                name: "FileName",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "TenTep");

            migrationBuilder.RenameColumn(
                name: "EntryId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "MaChungTu");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "LoaiNoiDung");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_EntryId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_MaChungTu");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_ActorId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Permission",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                newName: "Quyen");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                newName: "MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "Permission",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                newName: "Quyen");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Loai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "SucChua");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "HoatDong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_Code",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_MaSo");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "Lost",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongMat");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "MaPhieuMuon");

            migrationBuilder.RenameColumn(
                name: "Good",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongTot");

            migrationBuilder.RenameColumn(
                name: "Damaged",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongHong");

            migrationBuilder.RenameColumn(
                name: "At",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "AcceptedLost",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongMatNghiemThu");

            migrationBuilder.RenameColumn(
                name: "AcceptedGood",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongTotNghiemThu");

            migrationBuilder.RenameColumn(
                name: "AcceptedDamaged",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "SoLuongHongNghiemThu");

            migrationBuilder.RenameColumn(
                name: "AcceptedBy",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "MaNguoiNghiemThu");

            migrationBuilder.RenameColumn(
                name: "AcceptedAt",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "NgayNghiemThu");

            migrationBuilder.RenameColumn(
                name: "AcceptanceNote",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "GhiChuNghiemThu");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuTraThietBi_AcceptedBy",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "IX_PhieuTraThietBi_MaNguoiNghiemThu");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "MaNguoiDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewedAt",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "NgayDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "Reference",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "SoThamChieu");

            migrationBuilder.RenameColumn(
                name: "Reason",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "LyDo");

            migrationBuilder.RenameColumn(
                name: "Good",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "SoLuongTot");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Damaged",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "SoLuongHong");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_ReviewerId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_MaNguoiDuyet");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_DepartmentId_Status_CreatedAt",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_MaDonVi_TrangThai_NgayTao");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_AssetGroupId_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_MaNhomTaiSan_TrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_ActorId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "Start",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "BatDau");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Returned",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "SoLuongDaTra");

            migrationBuilder.RenameColumn(
                name: "ReportedReceived",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "SoLuongBaoDaNhan");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "MucDich");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "MaNguoiSoHuu");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "HandedAt",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "NgayBanGiao");

            migrationBuilder.RenameColumn(
                name: "End",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "KetThuc");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_OwnerId_CreatedAt",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_MaNguoiSoHuu_NgayTao");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_DepartmentId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_AssetGroupId_Status_Start_End",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_MaNhomTaiSan_TrangThai_BatDau_KetThuc");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "Start",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "BatDau");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MaPhong");

            migrationBuilder.RenameColumn(
                name: "ReviewedBy",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MaNguoiXetDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MucDich");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MaNguoiSoHuu");

            migrationBuilder.RenameColumn(
                name: "End",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "KetThuc");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Attendees",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "SoNguoiThamDu");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_RoomId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_MaPhong");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_OwnerId_CreatedAt",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_MaNguoiSoHuu_NgayTao");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_DepartmentId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "Start",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "BatDau");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Result",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "KetQua");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "Mode",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "CheDo");

            migrationBuilder.RenameColumn(
                name: "End",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "KetThuc");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Cost",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "ChiPhi");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "NoiDung");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_AssetGroupId_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_MaNhomTaiSan_TrangThai");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "NgayThuHoi");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "NgayHetHan");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhienDangNhap_UserId",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "IX_PhienDangNhap_MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "ProposalId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "MaDeXuat");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "DanhMuc");

            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "SoTien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_ProposalId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_MaDeXuat");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_DepartmentId_Year_Category",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_MaDonVi_Nam_DanhMuc");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameIndex(
                name: "IX_PhamViDonViNguoiDung_DepartmentId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "IX_PhamViDonViNguoiDung_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                newName: "NoiDung");

            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                newName: "MaTepDinhKem");

            migrationBuilder.RenameColumn(
                name: "Unit",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "DonViTinh");

            migrationBuilder.RenameColumn(
                name: "Total",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "TongSo");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "Lost",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "SoLuongMat");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Damaged",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "SoLuongHong");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "DanhMuc");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "HoatDong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_Code",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_MaSo");

            migrationBuilder.RenameColumn(
                name: "Used",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "DaSuDung");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "MaPhieuMuon");

            migrationBuilder.RenameColumn(
                name: "Issue",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "VanDe");

            migrationBuilder.RenameColumn(
                name: "Condition",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "TinhTrang");

            migrationBuilder.RenameColumn(
                name: "ActualStart",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "BatDauThucTe");

            migrationBuilder.RenameColumn(
                name: "ActualEnd",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "KetThucThucTe");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungThietBi_LoanId",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "IX_NhatKySuDungThietBi_MaPhieuMuon");

            migrationBuilder.RenameColumn(
                name: "Used",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "DaSuDung");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "ReviewedBy",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "MaNguoiXetDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "MaPhieuDangKy");

            migrationBuilder.RenameColumn(
                name: "Issue",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "VanDe");

            migrationBuilder.RenameColumn(
                name: "Condition",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "TinhTrang");

            migrationBuilder.RenameColumn(
                name: "Attendees",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "SoNguoiThamDu");

            migrationBuilder.RenameColumn(
                name: "ActualStart",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "BatDauThucTe");

            migrationBuilder.RenameColumn(
                name: "ActualEnd",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "KetThucThucTe");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungPhong_RequestId",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "IX_NhatKySuDungPhong_MaPhieuDangKy");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "MaDoiTuong");

            migrationBuilder.RenameColumn(
                name: "DetailJson",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ChiTietJson");

            migrationBuilder.RenameColumn(
                name: "At",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Action",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "HanhDong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKyHeThong_At",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "IX_NhatKyHeThong_ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TenDangNhap");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "BatXacThucHaiYeuTo");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DauBaoMat");

            migrationBuilder.RenameColumn(
                name: "Scope",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "PhamVi");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DaXacNhanSoDienThoai");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "SoDienThoai");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "MaBamMatKhau");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TenDangNhapChuanHoa");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "ThuDienTuChuanHoa");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "KhoaDen");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "ChoPhepKhoaTaiKhoan");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DaXacNhanThuDienTu");

            migrationBuilder.RenameColumn(
                name: "Email",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "ThuDienTu");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TenHienThi");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DauDongThoi");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "HoatDong");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "SoLanDangNhapSai");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_DepartmentId",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "IX_NguoiDung_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "GiaTri");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "NhaCungCapDangNhap");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Start",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "BatDau");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "MaPhong");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "MaPhieuDangKy");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "Kind",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "PhanLoai");

            migrationBuilder.RenameColumn(
                name: "End",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "KetThuc");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "HoatDong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_LichDatPhong_RoomId_Active_Start_End",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "IX_LichDatPhong_MaPhong_HoatDong_BatDau_KetThuc");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Reason",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "LyDo");

            migrationBuilder.RenameColumn(
                name: "Closed",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "DaKhoa");

            migrationBuilder.RenameColumn(
                name: "ChangedAt",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "NgayThayDoi");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_KyKeToan_ActorId",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "IX_KyKeToan_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "NgayCapNhat");

            migrationBuilder.RenameColumn(
                name: "Source",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "NguonGoc");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Reference",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "SoThamChieu");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "AcquisitionValue",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "GiaTriMua");

            migrationBuilder.RenameColumn(
                name: "AcquisitionDate",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "NgayMua");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameIndex(
                name: "IX_HoSoTaiSanCoDinh_ActorId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "IX_HoSoTaiSanCoDinh_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "At",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DotKiemKe_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "IX_DotKiemKe_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "Notes",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "GhiChuChung");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "TenChuanHoa");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "Active",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "HoatDong");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_NormalizedName",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_TenChuanHoa");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_Code",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_MaSo");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "MaNguoiDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "Reason",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "LyDo");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "DanhMuc");

            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "SoTien");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_ReviewerId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_MaNguoiDuyet");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_DepartmentId_Year_Category_Status",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_MaDonVi_Nam_DanhMuc_TrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_ActorId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "TieuDe");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "MaNguoiDuyet");

            migrationBuilder.RenameColumn(
                name: "ReviewNote",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "GhiChuDuyet");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "MucDich");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "MaNguoiSoHuu");

            migrationBuilder.RenameColumn(
                name: "LinesJson",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "DongChiTietJson");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "DanhMuc");

            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "SoTien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_ReviewerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_MaNguoiDuyet");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_OwnerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_MaNguoiSoHuu");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_DepartmentId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "PhienBanDong");

            migrationBuilder.RenameColumn(
                name: "MaintenanceId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "MaPhieuBaoTri");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "NoiDung");

            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "SoTien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DeNghiThanhToan_DepartmentId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "IX_DeNghiThanhToan_MaDonVi");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "TenHienThiNhaCungCap");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "KhoaNhaCungCap");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "NhaCungCapDangNhap");

            migrationBuilder.RenameIndex(
                name: "IX_DangNhapNgoai_UserId",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "IX_DangNhapNgoai_MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Year",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Nam");

            migrationBuilder.RenameColumn(
                name: "Voucher",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "SoChungTu");

            migrationBuilder.RenameColumn(
                name: "PaymentRequestId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "MaDeNghiThanhToan");

            migrationBuilder.RenameColumn(
                name: "NormalizedVoucher",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "SoChungTuChuanHoa");

            migrationBuilder.RenameColumn(
                name: "Kind",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "PhanLoai");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "MaDonVi");

            migrationBuilder.RenameColumn(
                name: "Date",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Ngay");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "Content",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "NoiDung");

            migrationBuilder.RenameColumn(
                name: "Category",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "DanhMuc");

            migrationBuilder.RenameColumn(
                name: "Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "SoTien");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_Year_NormalizedVoucher",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_Nam_SoChungTuChuanHoa");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_PaymentRequestId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_MaDeNghiThanhToan");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_DepartmentId_Year_Category_Kind",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_MaDonVi_Nam_DanhMuc_PhanLoai");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_ActorId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "ExpectedLost",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "SoLuongMatDuKien");

            migrationBuilder.RenameColumn(
                name: "ExpectedDamaged",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "SoLuongHongDuKien");

            migrationBuilder.RenameColumn(
                name: "Expected",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "SoLuongDuKien");

            migrationBuilder.RenameColumn(
                name: "Condition",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "TinhTrang");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "AssetVersion",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "PhienBanTaiSan");

            migrationBuilder.RenameColumn(
                name: "Actual",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "SoLuongThucTe");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "MaDotKiemKe");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietKiemKe_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "IX_ChiTietKiemKe_MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "Reason",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "LyDo");

            migrationBuilder.RenameColumn(
                name: "EntryId",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "MaChungTu");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_EntryId",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "IX_ButToanDao_MaChungTu");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_ActorId",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "IX_ButToanDao_MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "MaPhieuMuon");

            migrationBuilder.RenameColumn(
                name: "Kind",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "PhanLoai");

            migrationBuilder.RenameColumn(
                name: "At",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "MaNguoiThucHien");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_LoanId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_MaPhieuMuon");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_AssetGroupId_At",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_MaNhomTaiSan_ThoiDiem");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_ActorId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_MaNguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "TenChuanHoa",
                unique: true,
                filter: "[TenChuanHoa] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attachment_Size",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                sql: "[KichThuoc] BETWEEN 1 AND 2097152");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Room_Capacity",
                schema: "QuanLyTaiSan",
                table: "Phong",
                sql: "[SucChua] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuTraThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "MaPhieuMuon",
                unique: true,
                filter: "[NgayNghiemThu] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Return_Quantity",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                sql: "[SoLuongTot] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongMat] BETWEEN 0 AND 1000000 AND [SoLuongTot]+[SoLuongHong]+[SoLuongMat] BETWEEN 1 AND 1000000 AND (([NgayNghiemThu] IS NULL AND [SoLuongTotNghiemThu] IS NULL AND [SoLuongHongNghiemThu] IS NULL AND [SoLuongMatNghiemThu] IS NULL AND [MaNguoiNghiemThu] IS NULL) OR ([NgayNghiemThu] IS NOT NULL AND [MaNguoiNghiemThu] IS NOT NULL AND [SoLuongTotNghiemThu] IS NOT NULL AND [SoLuongHongNghiemThu] IS NOT NULL AND [SoLuongMatNghiemThu] IS NOT NULL AND [SoLuongTotNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongHongNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongMatNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongTotNghiemThu]+[SoLuongHongNghiemThu]+[SoLuongMatNghiemThu] BETWEEN 1 AND [SoLuongTot]+[SoLuongHong]+[SoLuongMat]))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetDisposal_Quantity",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                sql: "[SoLuongTot] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongTot]+[SoLuongHong] BETWEEN 1 AND 1000000");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetDisposal_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                sql: "([TrangThai] = 'pending' AND [MaNguoiDuyet] IS NULL AND [NgayDuyet] IS NULL) OR ([TrangThai] IN ('approved','rejected') AND [MaNguoiDuyet] IS NOT NULL AND [NgayDuyet] IS NOT NULL AND [MaNguoiDuyet] <> [MaNguoiThucHien])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Interval",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[BatDau] < [KetThuc]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Quantity",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[SoLuong] BETWEEN 1 AND 1000000 AND [SoLuongDaTra] >= 0 AND [SoLuongDaTra] <= [SoLuong] AND ([SoLuongBaoDaNhan] IS NULL OR [SoLuongBaoDaNhan] BETWEEN 0 AND [SoLuong])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Status",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[TrangThai] IN ('draft','pending','approved','rejected','cancelled','ready','receipt_disputed','borrowed','return_pending','completed') AND (([TrangThai] IN ('ready','receipt_disputed','borrowed','return_pending','completed') AND [NgayBanGiao] IS NOT NULL) OR ([TrangThai] IN ('draft','pending','approved','rejected','cancelled') AND [NgayBanGiao] IS NULL))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Attendees",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                sql: "[SoNguoiThamDu] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Interval",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                sql: "[BatDau] < [KetThuc]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Maintenance_Valid",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                sql: "[SoLuong] BETWEEN 1 AND 1000000 AND [BatDau] < [KetThuc] AND [CheDo] IN ('service','repair') AND [TrangThai] IN ('planned','running','completed','cancelled') AND [ChiPhi] BETWEEN 0 AND 1000000000000 AND (([TrangThai]='completed' AND [KetQua] IN ('good','unavailable')) OR ([TrangThai]<>'completed' AND [KetQua]=''))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Allocation_Amount",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                sql: "[SoTien] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AttachmentData_Size",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                sql: "DATALENGTH([NoiDung]) BETWEEN 1 AND 2097152");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Asset_Quantity",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                sql: "[TongSo] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongMat] BETWEEN 0 AND 1000000 AND [SoLuongHong] + [SoLuongMat] <= [TongSo]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EquipmentLog_Valid",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                sql: "[TrangThai] IN ('submitted','needs_changes','confirmed') AND (([DaSuDung]=0 AND [BatDauThucTe] IS NULL AND [KetThucThucTe] IS NULL AND [SoLuong] IS NULL) OR ([DaSuDung]=1 AND [BatDauThucTe] IS NOT NULL AND [KetThucThucTe] IS NOT NULL AND [SoLuong] IS NOT NULL AND [BatDauThucTe]<[KetThucThucTe] AND [SoLuong] BETWEEN 1 AND 1000000))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RoomLog_Actual",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                sql: "([DaSuDung] = 0 AND [BatDauThucTe] IS NULL AND [KetThucThucTe] IS NULL AND [SoNguoiThamDu] IS NULL) OR ([DaSuDung] = 1 AND [BatDauThucTe] IS NOT NULL AND [KetThucThucTe] IS NOT NULL AND [SoNguoiThamDu] IS NOT NULL AND [BatDauThucTe] < [KetThucThucTe] AND [SoNguoiThamDu] >= 0)");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "ThuDienTuChuanHoa",
                unique: true,
                filter: "[ThuDienTuChuanHoa] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "TenDangNhapChuanHoa",
                unique: true,
                filter: "[TenDangNhapChuanHoa] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LichDatPhong_MaPhieuDangKy",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "MaPhieuDangKy",
                unique: true,
                filter: "[MaPhieuDangKy] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservation_Interval",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                sql: "[BatDau] < [KetThuc]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FiscalPeriod_Year",
                schema: "KeToan",
                table: "KyKeToan",
                sql: "[Nam] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FixedAssetProfile_Value",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                sql: "[GiaTriMua] BETWEEN 1 AND 1000000000000 AND [NgayMua] >= '1900-01-01'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_Status",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                sql: "[TrangThai] IN ('open','completed')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Adjustment_Amount",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                sql: "[SoTien] <> 0 AND ABS([SoTien]) <= 1000000000000 AND [Nam] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Proposal_Amount",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                sql: "[SoTien] > 0 AND [Nam] BETWEEN 2000 AND 2100");

            migrationBuilder.CreateIndex(
                name: "IX_DeNghiThanhToan_MaPhieuBaoTri",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "MaPhieuBaoTri",
                unique: true,
                filter: "[MaPhieuBaoTri] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_Amount",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                sql: "[SoTien] > 0 AND [Nam] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                sql: "[SoTien] > 0 AND [Nam] BETWEEN 2000 AND 2100 AND YEAR([Ngay]) = [Nam] AND [PhanLoai] IN ('receipt','expense')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_Count",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                sql: "[SoLuongDuKien] BETWEEN 0 AND 1000000 AND [SoLuongHongDuKien]>=0 AND [SoLuongMatDuKien]>=0 AND [SoLuongHongDuKien]+[SoLuongMatDuKien]<=[SoLuongDuKien] AND ([SoLuongThucTe] IS NULL OR [SoLuongThucTe] BETWEEN 0 AND 1000000) AND [TinhTrang] IN ('','good','damaged','missing')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movement_Quantity",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                sql: "([PhanLoai] IN ('disposal.good','disposal.damaged') AND [SoLuong] BETWEEN -1000000 AND -1) OR ([PhanLoai] NOT IN ('disposal.good','disposal.damaged') AND [SoLuong] BETWEEN 1 AND 1000000)");

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "MaPhieuMuon",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ButToanDao",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_MaDotKiemKe",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                column: "MaDotKiemKe",
                principalSchema: "QuanLyTaiSan",
                principalTable: "DotKiemKe",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "MaDeNghiThanhToan",
                principalSchema: "KeToan",
                principalTable: "DeNghiThanhToan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_MaDonVi",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_MaDonVi",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaPhieuBaoTri",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "MaPhieuBaoTri",
                principalSchema: "QuanLyTaiSan",
                principalTable: "PhieuBaoTri",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKiemKe_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_DonVi_MaDonVi",
                schema: "KeToan",
                table: "KyKeToan",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_MaPhieuDangKy",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "MaPhieuDangKy",
                principalSchema: "GiangVien",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_Phong_MaPhong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "MaPhong",
                principalSchema: "QuanLyTaiSan",
                principalTable: "Phong",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_DonVi_MaDonVi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_MaPhieuDangKy",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                column: "MaPhieuDangKy",
                principalSchema: "GiangVien",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                column: "MaPhieuMuon",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhomTaiSan_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_MaTepDinhKem",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                column: "MaTepDinhKem",
                principalSchema: "KeToan",
                principalTable: "TepDinhKemChungTu",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_MaDonVi",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "MaDeXuat",
                principalSchema: "BanGiamHieu",
                principalTable: "DeXuatNganSach",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_MaDonVi",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_MaPhong",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "MaPhong",
                principalSchema: "QuanLyTaiSan",
                principalTable: "Phong",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_MaDonVi",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_MaNhomTaiSan",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaNhomTaiSan",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "MaNguoiNghiemThu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "MaPhieuMuon",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phong_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "Phong",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                column: "MaVaiTro",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                column: "MaVaiTro",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_MaNguoiDung_MaVaiTro",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                columns: new[] { "MaNguoiDung", "MaVaiTro" },
                principalSchema: "QuanTriVien",
                principalTable: "VaiTroNguoiDung",
                principalColumns: new[] { "MaNguoiDung", "MaVaiTro" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                column: "MaVaiTro",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_MaDotKiemKe",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_MaDonVi",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "DangNhapNgoai");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_MaDonVi",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaPhieuBaoTri",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKiemKe_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_DonVi_MaDonVi",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_MaPhieuDangKy",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_LichDatPhong_Phong_MaPhong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_DonVi_MaDonVi",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_MaPhieuDangKy",
                schema: "GiangVien",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_NhomTaiSan_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_MaTepDinhKem",
                schema: "KeToan",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_MaDonVi",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_MaDonVi",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_MaPhong",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_MaDonVi",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_MaNhomTaiSan",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_Phong_DonVi_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "Phong");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "QuyenVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro");

            migrationBuilder.DropForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_MaNguoiDung_MaVaiTro",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Attachment_Size",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Room_Capacity",
                schema: "QuanLyTaiSan",
                table: "Phong");

            migrationBuilder.DropIndex(
                name: "IX_PhieuTraThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Return_Quantity",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetDisposal_Quantity",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetDisposal_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Interval",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Quantity",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Status",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Attendees",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Interval",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Maintenance_Valid",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Allocation_Amount",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AttachmentData_Size",
                schema: "KeToan",
                table: "NoiDungTepChungTu");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Asset_Quantity",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EquipmentLog_Valid",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RoomLog_Actual",
                schema: "GiangVien",
                table: "NhatKySuDungPhong");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_LichDatPhong_MaPhieuDangKy",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservation_Interval",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FiscalPeriod_Year",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FixedAssetProfile_Value",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_Status",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Adjustment_Amount",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Proposal_Amount",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropIndex(
                name: "IX_DeNghiThanhToan_MaPhieuBaoTri",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payment_Amount",
                schema: "KeToan",
                table: "DeNghiThanhToan");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Inventory_Count",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movement_Quantity",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi");

            migrationBuilder.RenameColumn(
                name: "ThaoTac",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "Operation");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "MaBamNoiDung",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "PayloadHash");

            migrationBuilder.RenameColumn(
                name: "KetQuaJson",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "ResultJson");

            migrationBuilder.RenameColumn(
                name: "Khoa",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "YeuCauChongLap",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaVaiTro",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VaiTroNguoiDung_MaVaiTro",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                newName: "IX_VaiTroNguoiDung_RoleId");

            migrationBuilder.RenameColumn(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "PhamVi",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Scope");

            migrationBuilder.RenameColumn(
                name: "MacDinhHeThong",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "BuiltIn");

            migrationBuilder.RenameColumn(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PhamVi",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "Scope");

            migrationBuilder.RenameColumn(
                name: "DanhSachQuyenJson",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "PermissionsJson");

            migrationBuilder.RenameColumn(
                name: "MaVaiTro",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PhienBanNguon",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "SourceVersion");

            migrationBuilder.RenameColumn(
                name: "NgayDoc",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "KhoaNguon",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "SourceKey");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaVaiTro",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "LoaiThuocTinh",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "ClaimType");

            migrationBuilder.RenameColumn(
                name: "GiaTriThuocTinh",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhVaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                newName: "IX_ThuocTinhVaiTro_RoleId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "LoaiThuocTinh",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "ClaimType");

            migrationBuilder.RenameColumn(
                name: "GiaTriThuocTinh",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ThuocTinhNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                newName: "IX_ThuocTinhNguoiDung_UserId");

            migrationBuilder.RenameColumn(
                name: "TenTep",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "EntryId");

            migrationBuilder.RenameColumn(
                name: "MaBamSha256",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "Sha256");

            migrationBuilder.RenameColumn(
                name: "LoaiNoiDung",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "KichThuoc",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_TepDinhKemChungTu_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "IX_TepDinhKemChungTu_EntryId");

            migrationBuilder.RenameColumn(
                name: "Quyen",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                newName: "Permission");

            migrationBuilder.RenameColumn(
                name: "MaVaiTro",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "Quyen",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                newName: "Permission");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SucChua",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "Loai",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "HoatDong",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_MaSo",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_Code");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "At");

            migrationBuilder.RenameColumn(
                name: "SoLuongTotNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptedGood");

            migrationBuilder.RenameColumn(
                name: "SoLuongTot",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Good");

            migrationBuilder.RenameColumn(
                name: "SoLuongMatNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptedLost");

            migrationBuilder.RenameColumn(
                name: "SoLuongMat",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Lost");

            migrationBuilder.RenameColumn(
                name: "SoLuongHongNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptedDamaged");

            migrationBuilder.RenameColumn(
                name: "SoLuongHong",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Damaged");

            migrationBuilder.RenameColumn(
                name: "NgayNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptedAt");

            migrationBuilder.RenameColumn(
                name: "MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "LoanId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptedBy");

            migrationBuilder.RenameColumn(
                name: "GhiChuNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "AcceptanceNote");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuTraThietBi_MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "IX_PhieuTraThietBi_AcceptedBy");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoThamChieu",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Reference");

            migrationBuilder.RenameColumn(
                name: "SoLuongTot",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Good");

            migrationBuilder.RenameColumn(
                name: "SoLuongHong",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Damaged");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "NgayDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "ReviewedAt");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "ReviewerId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "LyDo",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_MaNhomTaiSan_TrangThai",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuThanhLyTaiSan_MaDonVi_TrangThai_NgayTao",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "IX_PhieuThanhLyTaiSan_DepartmentId_Status_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoLuongDaTra",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Returned");

            migrationBuilder.RenameColumn(
                name: "SoLuongBaoDaNhan",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "ReportedReceived");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "NgayBanGiao",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "HandedAt");

            migrationBuilder.RenameColumn(
                name: "MucDich",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "KetThuc",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "BatDau",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_MaNhomTaiSan_TrangThai_BatDau_KetThuc",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_AssetGroupId_Status_Start_End");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_MaNguoiSoHuu_NgayTao",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuMuonThietBi_MaDonVi",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "IX_PhieuMuonThietBi_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoNguoiThamDu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Attendees");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "MucDich",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "MaPhong",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiXetDuyet",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "ReviewedBy");

            migrationBuilder.RenameColumn(
                name: "MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "KetThuc",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "BatDau",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_MaPhong",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_MaNguoiSoHuu_NgayTao",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_OwnerId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuDangKyPhong_MaDonVi",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "IX_PhieuDangKyPhong_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NoiDung",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "KetThuc",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "KetQua",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "ChiPhi",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "CheDo",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Mode");

            migrationBuilder.RenameColumn(
                name: "BatDau",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_MaNhomTaiSan_TrangThai",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_AssetGroupId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "NgayThuHoi",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "NgayHetHan",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhienDangNhap_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "IX_PhienDangNhap_UserId");

            migrationBuilder.RenameColumn(
                name: "SoTien",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "ProposalId");

            migrationBuilder.RenameColumn(
                name: "DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_MaDonVi_Nam_DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_DepartmentId_Year_Category");

            migrationBuilder.RenameIndex(
                name: "IX_PhanBoNganSach_MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                newName: "IX_PhanBoNganSach_ProposalId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PhamViDonViNguoiDung_MaDonVi",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                newName: "IX_PhamViDonViNguoiDung_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "NoiDung",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "MaTepDinhKem",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                newName: "AttachmentId");

            migrationBuilder.RenameColumn(
                name: "TongSo",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SoLuongMat",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Lost");

            migrationBuilder.RenameColumn(
                name: "SoLuongHong",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Damaged");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "HoatDong",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "DonViTinh",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "DanhMuc",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_MaSo",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_Code");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "VanDe",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Issue");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TinhTrang",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Condition");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaPhieuMuon",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "LoanId");

            migrationBuilder.RenameColumn(
                name: "KetThucThucTe",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "ActualEnd");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "DaSuDung",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Used");

            migrationBuilder.RenameColumn(
                name: "BatDauThucTe",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "ActualStart");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungThietBi_MaPhieuMuon",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "IX_NhatKySuDungThietBi_LoanId");

            migrationBuilder.RenameColumn(
                name: "VanDe",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Issue");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TinhTrang",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Condition");

            migrationBuilder.RenameColumn(
                name: "SoNguoiThamDu",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Attendees");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaPhieuDangKy",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiXetDuyet",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "ReviewedBy");

            migrationBuilder.RenameColumn(
                name: "KetThucThucTe",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "ActualEnd");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "DaSuDung",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Used");

            migrationBuilder.RenameColumn(
                name: "BatDauThucTe",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "ActualStart");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKySuDungPhong_MaPhieuDangKy",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "IX_NhatKySuDungPhong_RequestId");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "At");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaDoiTuong",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "TargetId");

            migrationBuilder.RenameColumn(
                name: "HanhDong",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "ChiTietJson",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "DetailJson");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKyHeThong_ThoiDiem",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "IX_NhatKyHeThong_At");

            migrationBuilder.RenameColumn(
                name: "ThuDienTuChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "TenHienThi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "TenDangNhapChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "TenDangNhap",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "SoLanDangNhapSai",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "AccessFailedCount");

            migrationBuilder.RenameColumn(
                name: "SoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "PhamVi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Scope");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "MaBamMatKhau",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "KhoaDen",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "HoatDong",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "DauBaoMat",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "DaXacNhanThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "DaXacNhanSoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "ChoPhepKhoaTaiKhoan",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "BatXacThucHaiYeuTo",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_MaDonVi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "IX_NguoiDung_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "GiaTri",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NhaCungCapDangNhap",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "PhanLoai",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "MaPhong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "RoomId");

            migrationBuilder.RenameColumn(
                name: "MaPhieuDangKy",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "KetThuc",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "HoatDong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "BatDau",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_LichDatPhong_MaPhong_HoatDong_BatDau_KetThuc",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "IX_LichDatPhong_RoomId_Active_Start_End");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayThayDoi",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "ChangedAt");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "LyDo",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "DaKhoa",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "Closed");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_KyKeToan_MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan",
                newName: "IX_KyKeToan_ActorId");

            migrationBuilder.RenameColumn(
                name: "SoThamChieu",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "Reference");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NguonGoc",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "Source");

            migrationBuilder.RenameColumn(
                name: "NgayMua",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "AcquisitionDate");

            migrationBuilder.RenameColumn(
                name: "NgayCapNhat",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "GiaTriMua",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "AcquisitionValue");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "AssetGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_HoSoTaiSanCoDinh_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                newName: "IX_HoSoTaiSanCoDinh_ActorId");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "At");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DotKiemKe_MaDonVi",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "IX_DotKiemKe_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "HoatDong",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "GhiChuChung",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_MaSo",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_Code");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoTien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "ReviewerId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "LyDo",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "DanhMuc",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_DieuChinhNganSach_MaDonVi_Nam_DanhMuc_TrangThai",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "IX_DieuChinhNganSach_DepartmentId_Year_Category_Status");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TieuDe",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "SoTien",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MucDich",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "ReviewerId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "GhiChuDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "ReviewNote");

            migrationBuilder.RenameColumn(
                name: "DongChiTietJson",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "LinesJson");

            migrationBuilder.RenameColumn(
                name: "DanhMuc",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatNganSach_MaDonVi",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "IX_DeXuatNganSach_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SoTien",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PhienBanDong",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "NoiDung",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MaPhieuBaoTri",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "MaintenanceId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_DeNghiThanhToan_MaDonVi",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "IX_DeNghiThanhToan_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "TenHienThiNhaCungCap",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "KhoaNhaCungCap",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "NhaCungCapDangNhap",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "IX_DangNhapNgoai_MaNguoiDung",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                newName: "IX_DangNhapNgoai_UserId");

            migrationBuilder.RenameColumn(
                name: "SoTien",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "NormalizedVoucher");

            migrationBuilder.RenameColumn(
                name: "SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Voucher");

            migrationBuilder.RenameColumn(
                name: "PhanLoai",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "NoiDung",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Ngay",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Nam",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "PaymentRequestId");

            migrationBuilder.RenameColumn(
                name: "DanhMuc",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_Year_NormalizedVoucher");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_MaDonVi_Nam_DanhMuc_PhanLoai",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_DepartmentId_Year_Category_Kind");

            migrationBuilder.RenameIndex(
                name: "IX_ChungTuTaiChinh_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "IX_ChungTuTaiChinh_PaymentRequestId");

            migrationBuilder.RenameColumn(
                name: "TinhTrang",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Condition");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SoLuongThucTe",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Actual");

            migrationBuilder.RenameColumn(
                name: "SoLuongMatDuKien",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "ExpectedLost");

            migrationBuilder.RenameColumn(
                name: "SoLuongHongDuKien",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "ExpectedDamaged");

            migrationBuilder.RenameColumn(
                name: "SoLuongDuKien",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Expected");

            migrationBuilder.RenameColumn(
                name: "PhienBanTaiSan",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "AssetVersion");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "MaDotKiemKe",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietKiemKe_MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "IX_ChiTietKiemKe_AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MaChungTu",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "EntryId");

            migrationBuilder.RenameColumn(
                name: "LyDo",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_MaNguoiThucHien",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "IX_ButToanDao_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_ButToanDao_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "IX_ButToanDao_EntryId");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "At");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "PhanLoai",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "MaPhieuMuon",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "LoanId");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "AssetGroupId");

            migrationBuilder.RenameColumn(
                name: "MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_MaPhieuMuon",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_MaNhomTaiSan_ThoiDiem",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_AssetGroupId_At");

            migrationBuilder.RenameIndex(
                name: "IX_BienDongThietBi_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "IX_BienDongThietBi_ActorId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Attachment_Size",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                sql: "[Size] BETWEEN 1 AND 2097152");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Room_Capacity",
                schema: "QuanLyTaiSan",
                table: "Phong",
                sql: "[Capacity] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuTraThietBi_LoanId",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "LoanId",
                unique: true,
                filter: "[AcceptedAt] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Return_Quantity",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                sql: "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Good]+[Damaged]+[Lost] BETWEEN 1 AND 1000000 AND (([AcceptedAt] IS NULL AND [AcceptedGood] IS NULL AND [AcceptedDamaged] IS NULL AND [AcceptedLost] IS NULL AND [AcceptedBy] IS NULL) OR ([AcceptedAt] IS NOT NULL AND [AcceptedBy] IS NOT NULL AND [AcceptedGood] IS NOT NULL AND [AcceptedDamaged] IS NOT NULL AND [AcceptedLost] IS NOT NULL AND [AcceptedGood] BETWEEN 0 AND 1000000 AND [AcceptedDamaged] BETWEEN 0 AND 1000000 AND [AcceptedLost] BETWEEN 0 AND 1000000 AND [AcceptedGood]+[AcceptedDamaged]+[AcceptedLost] BETWEEN 1 AND [Good]+[Damaged]+[Lost]))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetDisposal_Quantity",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                sql: "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Good]+[Damaged] BETWEEN 1 AND 1000000");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetDisposal_Status",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                sql: "([Status] = 'pending' AND [ReviewerId] IS NULL AND [ReviewedAt] IS NULL) OR ([Status] IN ('approved','rejected') AND [ReviewerId] IS NOT NULL AND [ReviewedAt] IS NOT NULL AND [ReviewerId] <> [ActorId])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Interval",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[Start] < [End]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Quantity",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[Quantity] BETWEEN 1 AND 1000000 AND [Returned] >= 0 AND [Returned] <= [Quantity] AND ([ReportedReceived] IS NULL OR [ReportedReceived] BETWEEN 0 AND [Quantity])");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Status",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                sql: "[Status] IN ('draft','pending','approved','rejected','cancelled','ready','receipt_disputed','borrowed','return_pending','completed') AND (([Status] IN ('ready','receipt_disputed','borrowed','return_pending','completed') AND [HandedAt] IS NOT NULL) OR ([Status] IN ('draft','pending','approved','rejected','cancelled') AND [HandedAt] IS NULL))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Attendees",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                sql: "[Attendees] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Interval",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                sql: "[Start] < [End]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Maintenance_Valid",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                sql: "[Quantity] BETWEEN 1 AND 1000000 AND [Start] < [End] AND [Mode] IN ('service','repair') AND [Status] IN ('planned','running','completed','cancelled') AND [Cost] BETWEEN 0 AND 1000000000000 AND (([Status]='completed' AND [Result] IN ('good','unavailable')) OR ([Status]<>'completed' AND [Result]=''))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Allocation_Amount",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AttachmentData_Size",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                sql: "DATALENGTH([Content]) BETWEEN 1 AND 2097152");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Asset_Quantity",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                sql: "[Total] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Damaged] + [Lost] <= [Total]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EquipmentLog_Valid",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                sql: "[Status] IN ('submitted','needs_changes','confirmed') AND (([Used]=0 AND [ActualStart] IS NULL AND [ActualEnd] IS NULL AND [Quantity] IS NULL) OR ([Used]=1 AND [ActualStart] IS NOT NULL AND [ActualEnd] IS NOT NULL AND [Quantity] IS NOT NULL AND [ActualStart]<[ActualEnd] AND [Quantity] BETWEEN 1 AND 1000000))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RoomLog_Actual",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                sql: "([Used] = 0 AND [ActualStart] IS NULL AND [ActualEnd] IS NULL AND [Attendees] IS NULL) OR ([Used] = 1 AND [ActualStart] IS NOT NULL AND [ActualEnd] IS NOT NULL AND [Attendees] IS NOT NULL AND [ActualStart] < [ActualEnd] AND [Attendees] >= 0)");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LichDatPhong_RequestId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservation_Interval",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                sql: "[Start] < [End]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FiscalPeriod_Year",
                schema: "KeToan",
                table: "KyKeToan",
                sql: "[Year] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FixedAssetProfile_Value",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                sql: "[AcquisitionValue] BETWEEN 1 AND 1000000000000 AND [AcquisitionDate] >= '1900-01-01'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_Status",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                sql: "[Status] IN ('open','completed')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Adjustment_Amount",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                sql: "[Amount] <> 0 AND ABS([Amount]) <= 1000000000000 AND [Year] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Proposal_Amount",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                sql: "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100");

            migrationBuilder.CreateIndex(
                name: "IX_DeNghiThanhToan_MaintenanceId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "MaintenanceId",
                unique: true,
                filter: "[MaintenanceId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_Amount",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                sql: "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                sql: "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100 AND YEAR([Date]) = [Year] AND [Kind] IN ('receipt','expense')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Inventory_Count",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                sql: "[Expected] BETWEEN 0 AND 1000000 AND [ExpectedDamaged]>=0 AND [ExpectedLost]>=0 AND [ExpectedDamaged]+[ExpectedLost]<=[Expected] AND ([Actual] IS NULL OR [Actual] BETWEEN 0 AND 1000000) AND [Condition] IN ('','good','damaged','missing')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movement_Quantity",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                sql: "([Kind] IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN -1000000 AND -1) OR ([Kind] NOT IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN 1 AND 1000000)");

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_PhieuMuonThietBi_LoanId",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "LoanId",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_EntryId",
                schema: "KeToan",
                table: "ButToanDao",
                column: "EntryId",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_NguoiDung_ActorId",
                schema: "KeToan",
                table: "ButToanDao",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_DotKiemKe_SessionId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                column: "SessionId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "DotKiemKe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietKiemKe_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DeNghiThanhToan_PaymentRequestId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "PaymentRequestId",
                principalSchema: "KeToan",
                principalTable: "DeNghiThanhToan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_DonVi_DepartmentId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_NguoiDung_ActorId",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DangNhapNgoai_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_DonVi_DepartmentId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeNghiThanhToan_PhieuBaoTri_MaintenanceId",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                column: "MaintenanceId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "PhieuBaoTri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_OwnerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "OwnerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_ReviewerId",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "ReviewerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ActorId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_ReviewerId",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "ReviewerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKiemKe_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_DonVi_DepartmentId",
                schema: "KeToan",
                table: "KyKeToan",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_NguoiDung_ActorId",
                schema: "KeToan",
                table: "KyKeToan",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_PhieuDangKyPhong_RequestId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "RequestId",
                principalSchema: "GiangVien",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LichDatPhong_Phong_RoomId",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                column: "RoomId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "Phong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaXacThucNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "MaXacThucNguoiDung",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_DonVi_DepartmentId",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungPhong_PhieuDangKyPhong_RequestId",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                column: "RequestId",
                principalSchema: "GiangVien",
                principalTable: "PhieuDangKyPhong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhatKySuDungThietBi_PhieuMuonThietBi_LoanId",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                column: "LoanId",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NhomTaiSan_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NoiDungTepChungTu_TepDinhKemChungTu_AttachmentId",
                schema: "KeToan",
                table: "NoiDungTepChungTu",
                column: "AttachmentId",
                principalSchema: "KeToan",
                principalTable: "TepDinhKemChungTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_DonVi_DepartmentId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DeXuatNganSach_ProposalId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "ProposalId",
                principalSchema: "BanGiamHieu",
                principalTable: "DeXuatNganSach",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DonVi_DepartmentId",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_DonVi_DepartmentId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_OwnerId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "OwnerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_Phong_RoomId",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "RoomId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "Phong",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_DonVi_DepartmentId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_OwnerId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "OwnerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_NhomTaiSan_AssetGroupId",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ActorId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_ReviewerId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "ReviewerId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NhomTaiSan_AssetGroupId",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "AssetGroupId",
                principalSchema: "QuanLyTaiSan",
                principalTable: "NhomTaiSan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_AcceptedBy",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "AcceptedBy",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_PhieuMuonThietBi_LoanId",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "LoanId",
                principalSchema: "GiangVien",
                principalTable: "PhieuMuonThietBi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phong_DonVi_DepartmentId",
                schema: "QuanLyTaiSan",
                table: "Phong",
                column: "DepartmentId",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenVaiTro_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "QuyenVaiTro",
                column: "RoleId",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_EntryId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "EntryId",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_ActorId",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThuocTinhVaiTro_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                column: "RoleId",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_ActorId",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                column: "ActorId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TuyChinhVaiTroNguoiDung_VaiTroNguoiDung_UserId_RoleId",
                schema: "QuanTriVien",
                table: "TuyChinhVaiTroNguoiDung",
                columns: new[] { "UserId", "RoleId" },
                principalSchema: "QuanTriVien",
                principalTable: "VaiTroNguoiDung",
                principalColumns: new[] { "UserId", "RoleId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_UserId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                column: "UserId",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_VaiTro_RoleId",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                column: "RoleId",
                principalSchema: "QuanTriVien",
                principalTable: "VaiTro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
