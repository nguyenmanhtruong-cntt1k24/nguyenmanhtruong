using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRequestedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaiTro_Ten",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.RenameColumn(
                name: "Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "TenVaiTro");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "MaVaiTro");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "MaTepDinhKem");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "MaSoPhong");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "MaPhong");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_MaSo",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_MaSoPhong");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "MaPhieuTraThietBi");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "MaPhieuThanhLyTaiSan");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "MaPhieuMuon");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "MaPhieuDangKy");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "MaPhieuBaoTri");

            migrationBuilder.RenameColumn(
                name: "NgayHetHan",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "NgayHetPhien");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "MaPhienDangNhap");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "MaSoNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "MaNhomTaiSan");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_MaSo",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_MaSoNhomTaiSan");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "MaNhatKySuDungThietBi");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "MaNhatKySuDungPhong");

            migrationBuilder.RenameColumn(
                name: "ThoiDiem",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ThoiDien");

            migrationBuilder.RenameColumn(
                name: "ChiTietJson",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ChiTiet");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "MaNhatKyHeThong");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKyHeThong_ThoiDiem",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "IX_NhatKyHeThong_ThoiDien");

            migrationBuilder.RenameColumn(
                name: "ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "DiaChiEmail");

            migrationBuilder.RenameColumn(
                name: "TenHienThi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TenNguoiDung");

            migrationBuilder.RenameColumn(
                name: "MaBamMatKhau",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "MatKhau");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "MaNguoiDung");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "MaLichDatPhong");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "MaDotKiemKe");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "MaSoDonVi");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "MaDonVi");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_MaSo",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_MaSoDonVi");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "MaDieuChinhNganSach");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "MaDeXuat");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "MaDeNghiThanhToan");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "MaChungTu");

            migrationBuilder.RenameColumn(
                name: "MaSo",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "MaSoChiTietKiemKe");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "MaButToanDao");

            migrationBuilder.RenameColumn(
                name: "Ma",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "MaBienDongThietBi");

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgayThangNamSinh",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "date",
                nullable: true);

            migrationBuilder.Sql("""
                EXEC(N'CREATE FUNCTION [QuanTriVien].[TenDonViNguoiDung] (@MaDonVi nvarchar(450))
                RETURNS nvarchar(150)
                AS BEGIN
                    RETURN (SELECT [Ten] FROM [QuanTriVien].[DonVi] WHERE [MaDonVi] = @MaDonVi);
                END');
                """);

            migrationBuilder.AddColumn<string>(
                name: "DonVi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                computedColumnSql: "[QuanTriVien].[TenDonViNguoiDung]([MaDonVi])",
                stored: false);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true,
                filter: "[TenVaiTro] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_DiaChiEmail",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "DiaChiEmail",
                unique: true,
                filter: "[DiaChiEmail] IS NOT NULL");
            migrationBuilder.Sql("""
                EXEC(N'CREATE VIEW [QuanTriVien].[PhanQuyen] AS
                SELECT DISTINCT r.[MaVaiTro], r.[TenVaiTro], ur.[MaNguoiDung], p.[Quyen]
                FROM [QuanTriVien].[VaiTroNguoiDung] ur
                JOIN [QuanTriVien].[VaiTro] r ON r.[MaVaiTro] = ur.[MaVaiTro]
                LEFT JOIN [QuanTriVien].[TuyChinhVaiTroNguoiDung] o
                    ON o.[MaVaiTro] = ur.[MaVaiTro] AND o.[MaNguoiDung] = ur.[MaNguoiDung]
                CROSS APPLY (
                    SELECT rp.[Quyen] FROM [QuanTriVien].[QuyenVaiTro] rp
                    WHERE rp.[MaVaiTro] = ur.[MaVaiTro] AND o.[MaNguoiDung] IS NULL
                    UNION ALL
                    SELECT CONVERT(nvarchar(100), j.[value])
                    FROM OPENJSON(o.[DanhSachQuyenJson]) j WHERE o.[MaNguoiDung] IS NOT NULL
                ) p;');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rolling back would discard newly entered dates of birth.
            migrationBuilder.Sql("THROW 51030, 'Fix schema rollback requires a reviewed data-preserving migration.', 1;");
            migrationBuilder.DropIndex(
                name: "IX_VaiTro_TenVaiTro",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_DiaChiEmail",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DonVi",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "NgayThangNamSinh",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.RenameColumn(
                name: "TenVaiTro",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Ten");

            migrationBuilder.RenameColumn(
                name: "MaVaiTro",
                schema: "QuanTriVien",
                table: "VaiTro",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaTepDinhKem",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaSoPhong",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "MaPhong",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_Phong_MaSoPhong",
                schema: "QuanLyTaiSan",
                table: "Phong",
                newName: "IX_Phong_MaSo");

            migrationBuilder.RenameColumn(
                name: "MaPhieuTraThietBi",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaPhieuThanhLyTaiSan",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaPhieuMuon",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaPhieuDangKy",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaPhieuBaoTri",
                schema: "QuanLyTaiSan",
                table: "PhieuBaoTri",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "NgayHetPhien",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "NgayHetHan");

            migrationBuilder.RenameColumn(
                name: "MaPhienDangNhap",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaSoNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "MaNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhomTaiSan_MaSoNhomTaiSan",
                schema: "QuanLyTaiSan",
                table: "NhomTaiSan",
                newName: "IX_NhomTaiSan_MaSo");

            migrationBuilder.RenameColumn(
                name: "MaNhatKySuDungThietBi",
                schema: "GiangVien",
                table: "NhatKySuDungThietBi",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaNhatKySuDungPhong",
                schema: "GiangVien",
                table: "NhatKySuDungPhong",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "ThoiDien",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "ChiTiet",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "ChiTietJson");

            migrationBuilder.RenameColumn(
                name: "MaNhatKyHeThong",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_NhatKyHeThong_ThoiDien",
                schema: "QuanTriVien",
                table: "NhatKyHeThong",
                newName: "IX_NhatKyHeThong_ThoiDiem");

            migrationBuilder.RenameColumn(
                name: "TenNguoiDung",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "TenHienThi");

            migrationBuilder.RenameColumn(
                name: "MatKhau",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "MaBamMatKhau");

            migrationBuilder.RenameColumn(
                name: "DiaChiEmail",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "ThuDienTu");

            migrationBuilder.RenameColumn(
                name: "MaNguoiDung",
                schema: "QuanTriVien",
                table: "NguoiDung",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaLichDatPhong",
                schema: "QuanLyTaiSan",
                table: "LichDatPhong",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaDotKiemKe",
                schema: "QuanLyTaiSan",
                table: "DotKiemKe",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaSoDonVi",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "MaDonVi",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "Ma");

            migrationBuilder.RenameIndex(
                name: "IX_DonVi_MaSoDonVi",
                schema: "QuanTriVien",
                table: "DonVi",
                newName: "IX_DonVi_MaSo");

            migrationBuilder.RenameColumn(
                name: "MaDieuChinhNganSach",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaDeXuat",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaDeNghiThanhToan",
                schema: "KeToan",
                table: "DeNghiThanhToan",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaSoChiTietKiemKe",
                schema: "QuanLyTaiSan",
                table: "ChiTietKiemKe",
                newName: "MaSo");

            migrationBuilder.RenameColumn(
                name: "MaButToanDao",
                schema: "KeToan",
                table: "ButToanDao",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "MaBienDongThietBi",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                newName: "Ma");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "Ten",
                unique: true,
                filter: "[Ten] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "ThuDienTu",
                unique: true,
                filter: "[ThuDienTu] IS NOT NULL");
        }
    }
}
