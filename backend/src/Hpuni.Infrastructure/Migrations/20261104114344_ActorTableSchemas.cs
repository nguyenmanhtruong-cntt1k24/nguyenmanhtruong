using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActorTableSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "QuanLyTaiSan");

            migrationBuilder.EnsureSchema(
                name: "KeToan");

            migrationBuilder.EnsureSchema(
                name: "QuanTriVien");

            migrationBuilder.EnsureSchema(
                name: "BanGiamHieu");

            migrationBuilder.EnsureSchema(
                name: "GiangVien");

            migrationBuilder.RenameTable(
                name: "YeuCauChongLap",
                newName: "YeuCauChongLap",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "VaiTroNguoiDung",
                newName: "VaiTroNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "VaiTro",
                newName: "VaiTro",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "TuyChinhVaiTroNguoiDung",
                newName: "TuyChinhVaiTroNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "TrangThaiDocThongBao",
                newName: "TrangThaiDocThongBao",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "ThuocTinhVaiTro",
                newName: "ThuocTinhVaiTro",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "ThuocTinhNguoiDung",
                newName: "ThuocTinhNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "TepDinhKemChungTu",
                newName: "TepDinhKemChungTu",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "QuyenVaiTro",
                newName: "QuyenVaiTro",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "QuyenBiTuChoiNguoiDung",
                newName: "QuyenBiTuChoiNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "Phong",
                newName: "Phong",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "PhieuTraThietBi",
                newName: "PhieuTraThietBi",
                newSchema: "GiangVien");

            migrationBuilder.RenameTable(
                name: "PhieuThanhLyTaiSan",
                newName: "PhieuThanhLyTaiSan",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "PhieuMuonThietBi",
                newName: "PhieuMuonThietBi",
                newSchema: "GiangVien");

            migrationBuilder.RenameTable(
                name: "PhieuDangKyPhong",
                newName: "PhieuDangKyPhong",
                newSchema: "GiangVien");

            migrationBuilder.RenameTable(
                name: "PhieuBaoTri",
                newName: "PhieuBaoTri",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "PhienDangNhap",
                newName: "PhienDangNhap",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "PhanBoNganSach",
                newName: "PhanBoNganSach",
                newSchema: "BanGiamHieu");

            migrationBuilder.RenameTable(
                name: "PhamViDonViNguoiDung",
                newName: "PhamViDonViNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "NoiDungTepChungTu",
                newName: "NoiDungTepChungTu",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "NhomTaiSan",
                newName: "NhomTaiSan",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungThietBi",
                newName: "NhatKySuDungThietBi",
                newSchema: "GiangVien");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungPhong",
                newName: "NhatKySuDungPhong",
                newSchema: "GiangVien");

            migrationBuilder.RenameTable(
                name: "NhatKyHeThong",
                newName: "NhatKyHeThong",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                newName: "NguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "MaXacThucNguoiDung",
                newName: "MaXacThucNguoiDung",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "LichDatPhong",
                newName: "LichDatPhong",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "KyKeToan",
                newName: "KyKeToan",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "HoSoTaiSanCoDinh",
                newName: "HoSoTaiSanCoDinh",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "DotKiemKe",
                newName: "DotKiemKe",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "DonVi",
                newName: "DonVi",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "DieuChinhNganSach",
                newName: "DieuChinhNganSach",
                newSchema: "BanGiamHieu");

            migrationBuilder.RenameTable(
                name: "DeXuatNganSach",
                newName: "DeXuatNganSach",
                newSchema: "BanGiamHieu");

            migrationBuilder.RenameTable(
                name: "DeNghiThanhToan",
                newName: "DeNghiThanhToan",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "DangNhapNgoai",
                newName: "DangNhapNgoai",
                newSchema: "QuanTriVien");

            migrationBuilder.RenameTable(
                name: "ChungTuTaiChinh",
                newName: "ChungTuTaiChinh",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "ChiTietKiemKe",
                newName: "ChiTietKiemKe",
                newSchema: "QuanLyTaiSan");

            migrationBuilder.RenameTable(
                name: "ButToanDao",
                newName: "ButToanDao",
                newSchema: "KeToan");

            migrationBuilder.RenameTable(
                name: "BienDongThietBi",
                newName: "BienDongThietBi",
                newSchema: "QuanLyTaiSan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "YeuCauChongLap",
                schema: "QuanTriVien",
                newName: "YeuCauChongLap");

            migrationBuilder.RenameTable(
                name: "VaiTroNguoiDung",
                schema: "QuanTriVien",
                newName: "VaiTroNguoiDung");

            migrationBuilder.RenameTable(
                name: "VaiTro",
                schema: "QuanTriVien",
                newName: "VaiTro");

            migrationBuilder.RenameTable(
                name: "TuyChinhVaiTroNguoiDung",
                schema: "QuanTriVien",
                newName: "TuyChinhVaiTroNguoiDung");

            migrationBuilder.RenameTable(
                name: "TrangThaiDocThongBao",
                schema: "QuanTriVien",
                newName: "TrangThaiDocThongBao");

            migrationBuilder.RenameTable(
                name: "ThuocTinhVaiTro",
                schema: "QuanTriVien",
                newName: "ThuocTinhVaiTro");

            migrationBuilder.RenameTable(
                name: "ThuocTinhNguoiDung",
                schema: "QuanTriVien",
                newName: "ThuocTinhNguoiDung");

            migrationBuilder.RenameTable(
                name: "TepDinhKemChungTu",
                schema: "KeToan",
                newName: "TepDinhKemChungTu");

            migrationBuilder.RenameTable(
                name: "QuyenVaiTro",
                schema: "QuanTriVien",
                newName: "QuyenVaiTro");

            migrationBuilder.RenameTable(
                name: "QuyenBiTuChoiNguoiDung",
                schema: "QuanTriVien",
                newName: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.RenameTable(
                name: "Phong",
                schema: "QuanLyTaiSan",
                newName: "Phong");

            migrationBuilder.RenameTable(
                name: "PhieuTraThietBi",
                schema: "GiangVien",
                newName: "PhieuTraThietBi");

            migrationBuilder.RenameTable(
                name: "PhieuThanhLyTaiSan",
                schema: "QuanLyTaiSan",
                newName: "PhieuThanhLyTaiSan");

            migrationBuilder.RenameTable(
                name: "PhieuMuonThietBi",
                schema: "GiangVien",
                newName: "PhieuMuonThietBi");

            migrationBuilder.RenameTable(
                name: "PhieuDangKyPhong",
                schema: "GiangVien",
                newName: "PhieuDangKyPhong");

            migrationBuilder.RenameTable(
                name: "PhieuBaoTri",
                schema: "QuanLyTaiSan",
                newName: "PhieuBaoTri");

            migrationBuilder.RenameTable(
                name: "PhienDangNhap",
                schema: "QuanTriVien",
                newName: "PhienDangNhap");

            migrationBuilder.RenameTable(
                name: "PhanBoNganSach",
                schema: "BanGiamHieu",
                newName: "PhanBoNganSach");

            migrationBuilder.RenameTable(
                name: "PhamViDonViNguoiDung",
                schema: "QuanTriVien",
                newName: "PhamViDonViNguoiDung");

            migrationBuilder.RenameTable(
                name: "NoiDungTepChungTu",
                schema: "KeToan",
                newName: "NoiDungTepChungTu");

            migrationBuilder.RenameTable(
                name: "NhomTaiSan",
                schema: "QuanLyTaiSan",
                newName: "NhomTaiSan");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungThietBi",
                schema: "GiangVien",
                newName: "NhatKySuDungThietBi");

            migrationBuilder.RenameTable(
                name: "NhatKySuDungPhong",
                schema: "GiangVien",
                newName: "NhatKySuDungPhong");

            migrationBuilder.RenameTable(
                name: "NhatKyHeThong",
                schema: "QuanTriVien",
                newName: "NhatKyHeThong");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                schema: "QuanTriVien",
                newName: "NguoiDung");

            migrationBuilder.RenameTable(
                name: "MaXacThucNguoiDung",
                schema: "QuanTriVien",
                newName: "MaXacThucNguoiDung");

            migrationBuilder.RenameTable(
                name: "LichDatPhong",
                schema: "QuanLyTaiSan",
                newName: "LichDatPhong");

            migrationBuilder.RenameTable(
                name: "KyKeToan",
                schema: "KeToan",
                newName: "KyKeToan");

            migrationBuilder.RenameTable(
                name: "HoSoTaiSanCoDinh",
                schema: "QuanLyTaiSan",
                newName: "HoSoTaiSanCoDinh");

            migrationBuilder.RenameTable(
                name: "DotKiemKe",
                schema: "QuanLyTaiSan",
                newName: "DotKiemKe");

            migrationBuilder.RenameTable(
                name: "DonVi",
                schema: "QuanTriVien",
                newName: "DonVi");

            migrationBuilder.RenameTable(
                name: "DieuChinhNganSach",
                schema: "BanGiamHieu",
                newName: "DieuChinhNganSach");

            migrationBuilder.RenameTable(
                name: "DeXuatNganSach",
                schema: "BanGiamHieu",
                newName: "DeXuatNganSach");

            migrationBuilder.RenameTable(
                name: "DeNghiThanhToan",
                schema: "KeToan",
                newName: "DeNghiThanhToan");

            migrationBuilder.RenameTable(
                name: "DangNhapNgoai",
                schema: "QuanTriVien",
                newName: "DangNhapNgoai");

            migrationBuilder.RenameTable(
                name: "ChungTuTaiChinh",
                schema: "KeToan",
                newName: "ChungTuTaiChinh");

            migrationBuilder.RenameTable(
                name: "ChiTietKiemKe",
                schema: "QuanLyTaiSan",
                newName: "ChiTietKiemKe");

            migrationBuilder.RenameTable(
                name: "ButToanDao",
                schema: "KeToan",
                newName: "ButToanDao");

            migrationBuilder.RenameTable(
                name: "BienDongThietBi",
                schema: "QuanLyTaiSan",
                newName: "BienDongThietBi");
        }
    }
}
