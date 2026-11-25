using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExactFixSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [QuanTriVien].[PhienDangNhap] GROUP BY [MaNguoiDung],[NgayTao] HAVING COUNT(*)>1)
                THROW 51041, 'Duplicate session creation timestamps; migration refused.', 1;
                SELECT [MaNguoiDung],[PhamVi],[HoatDong],[DauBaoMat],[KhoaDen],[ChoPhepKhoaTaiKhoan],[SoLanDangNhapSai]
                INTO #ExactUser FROM [QuanTriVien].[NguoiDung];
                SELECT [MaNguoiDung],[NgayTao],[MaPhienDangNhap],[NgayThuHoi] INTO #ExactSession FROM [QuanTriVien].[PhienDangNhap];
                SELECT [MaNhatKyHeThong],[MaDoiTuong] INTO #ExactAudit FROM [QuanTriVien].[NhatKyHeThong];
                SELECT [MaChungTu],[MaDonVi],[Nam],[DanhMuc],[SoChungTu],[Ngay],[MaDeNghiThanhToan]
                INTO #ExactFinance FROM [KeToan].[ChungTuTaiChinh];
                SELECT [MaVaiTro],[TenVaiTro],[MaNguoiDung],[Quyen] INTO #ExactPermission FROM [QuanTriVien].[PhanQuyen];
                DROP VIEW [QuanTriVien].[PhanQuyen];
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_BienDongThietBi_NguoiDung_MaNguoiThucHien",
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
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatNganSach_NguoiDung_MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_DieuChinhNganSach_NguoiDung_MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach");

            migrationBuilder.DropForeignKey(
                name: "FK_HoSoTaiSanCoDinh_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh");

            migrationBuilder.DropForeignKey(
                name: "FK_KyKeToan_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhamViDonViNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_PhienDangNhap_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuDangKyPhong_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuMuonThietBi_NguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThanhLyTaiSan_NguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuTraThietBi_NguoiDung_MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi");

            migrationBuilder.DropForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_NguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_TrangThaiDocThongBao_NguoiDung_MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao");

            migrationBuilder.DropForeignKey(
                name: "FK_VaiTroNguoiDung_NguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhienDangNhap",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_MaDonVi_Nam_DanhMuc_PhanLoai",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "MaPhienDangNhap",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropColumn(
                name: "NgayThuHoi",
                schema: "QuanTriVien",
                table: "PhienDangNhap");

            migrationBuilder.DropColumn(
                name: "MaDoiTuong",
                schema: "QuanTriVien",
                table: "NhatKyHeThong");

            migrationBuilder.DropColumn(
                name: "ChoPhepKhoaTaiKhoan",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DauBaoMat",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "HoatDong",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "KhoaDen",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "PhamVi",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "PhienBanDong",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "SoLanDangNhapSai",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DanhMuc",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "MaDonVi",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "Nam",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "Ngay",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhienDangNhap",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                columns: new[] { "MaNguoiDung", "NgayTao" });

            migrationBuilder.CreateTable(
                name: "BaoMatNguoiDung",
                schema: "QuanTriVien",
                columns: table => new
                {
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhamVi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HoatDong = table.Column<bool>(type: "bit", nullable: false),
                    PhienBanDong = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DauBaoMat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhoaDen = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ChoPhepKhoaTaiKhoan = table.Column<bool>(type: "bit", nullable: false),
                    SoLanDangNhapSai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoMatNguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_BaoMatNguoiDung_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalSchema: "QuanTriVien",
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietChungTu",
                schema: "KeToan",
                columns: table => new
                {
                    MaChungTu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaDonVi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    DanhMuc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SoChungTu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AS"),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaDeNghiThanhToan = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietChungTu", x => x.MaChungTu);
                    table.ForeignKey(
                        name: "FK_ChiTietChungTu_ChungTuTaiChinh_MaChungTu",
                        column: x => x.MaChungTu,
                        principalSchema: "KeToan",
                        principalTable: "ChungTuTaiChinh",
                        principalColumn: "MaChungTu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoiTuongNhatKy",
                schema: "QuanTriVien",
                columns: table => new
                {
                    MaNhatKyHeThong = table.Column<long>(type: "bigint", nullable: false),
                    MaDoiTuong = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoiTuongNhatKy", x => x.MaNhatKyHeThong);
                    table.ForeignKey(
                        name: "FK_DoiTuongNhatKy_NhatKyHeThong_MaNhatKyHeThong",
                        column: x => x.MaNhatKyHeThong,
                        principalSchema: "QuanTriVien",
                        principalTable: "NhatKyHeThong",
                        principalColumn: "MaNhatKyHeThong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                schema: "QuanTriVien",
                columns: table => new
                {
                    MaVaiTro = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Quyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenVaiTro = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, collation: "Latin1_General_100_CI_AS")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyen", x => new { x.MaNguoiDung, x.MaVaiTro, x.Quyen });
                });

            migrationBuilder.CreateTable(
                name: "DinhDanhPhienDangNhap",
                schema: "QuanTriVien",
                columns: table => new
                {
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MaPhienDangNhap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayThuHoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhDanhPhienDangNhap", x => new { x.MaNguoiDung, x.NgayTao });
                    table.ForeignKey(
                        name: "FK_DinhDanhPhienDangNhap_BaoMatNguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalSchema: "QuanTriVien",
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DinhDanhPhienDangNhap_PhienDangNhap_MaNguoiDung_NgayTao",
                        columns: x => new { x.MaNguoiDung, x.NgayTao },
                        principalSchema: "QuanTriVien",
                        principalTable: "PhienDangNhap",
                        principalColumns: new[] { "MaNguoiDung", "NgayTao" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT [QuanTriVien].[BaoMatNguoiDung] ([MaNguoiDung],[PhamVi],[HoatDong],[DauBaoMat],[KhoaDen],[ChoPhepKhoaTaiKhoan],[SoLanDangNhapSai])
                SELECT [MaNguoiDung],[PhamVi],[HoatDong],[DauBaoMat],[KhoaDen],[ChoPhepKhoaTaiKhoan],[SoLanDangNhapSai] FROM #ExactUser;
                INSERT [QuanTriVien].[DinhDanhPhienDangNhap] ([MaNguoiDung],[NgayTao],[MaPhienDangNhap],[NgayThuHoi])
                SELECT [MaNguoiDung],[NgayTao],[MaPhienDangNhap],[NgayThuHoi] FROM #ExactSession;
                INSERT [QuanTriVien].[DoiTuongNhatKy] ([MaNhatKyHeThong],[MaDoiTuong]) SELECT * FROM #ExactAudit;
                INSERT [KeToan].[ChiTietChungTu] ([MaChungTu],[MaDonVi],[Nam],[DanhMuc],[SoChungTu],[Ngay],[MaDeNghiThanhToan]) SELECT * FROM #ExactFinance;
                INSERT [QuanTriVien].[PhanQuyen] ([MaVaiTro],[TenVaiTro],[MaNguoiDung],[Quyen]) SELECT * FROM #ExactPermission;
                ALTER TABLE [KeToan].[ChiTietChungTu] ADD CONSTRAINT [FK_ChiTietChungTu_DonVi] FOREIGN KEY ([MaDonVi]) REFERENCES [QuanTriVien].[DonVi]([MaDonVi]);
                ALTER TABLE [KeToan].[ChiTietChungTu] ADD CONSTRAINT [FK_ChiTietChungTu_DeNghiThanhToan] FOREIGN KEY ([MaDeNghiThanhToan]) REFERENCES [KeToan].[DeNghiThanhToan]([MaDeNghiThanhToan]);
                ALTER TABLE [KeToan].[ChiTietChungTu] ADD CONSTRAINT [CK_Entry_Period] CHECK ([Nam] BETWEEN 2000 AND 2100 AND YEAR([Ngay])=[Nam]);
                ALTER TABLE [QuanTriVien].[PhanQuyen] ADD CONSTRAINT [FK_PhanQuyen_VaiTroNguoiDung] FOREIGN KEY ([MaNguoiDung],[MaVaiTro]) REFERENCES [QuanTriVien].[VaiTroNguoiDung]([MaNguoiDung],[MaVaiTro]) ON DELETE CASCADE;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuTaiChinh_PhanLoai",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "PhanLoai");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                sql: "[SoTien] > 0 AND [PhanLoai] IN ('receipt','expense')");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChungTu_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChiTietChungTu",
                column: "MaDeNghiThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChungTu_MaDonVi_Nam_DanhMuc",
                schema: "KeToan",
                table: "ChiTietChungTu",
                columns: new[] { "MaDonVi", "Nam", "DanhMuc" });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChungTu_Nam_SoChungTu",
                schema: "KeToan",
                table: "ChiTietChungTu",
                columns: new[] { "Nam", "SoChungTu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DinhDanhPhienDangNhap_MaNguoiDung",
                schema: "QuanTriVien",
                table: "DinhDanhPhienDangNhap",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DinhDanhPhienDangNhap_MaPhienDangNhap",
                schema: "QuanTriVien",
                table: "DinhDanhPhienDangNhap",
                column: "MaPhienDangNhap",
                unique: true,
                filter: "[MaPhienDangNhap] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BienDongThietBi_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "BienDongThietBi",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ButToanDao",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_ChiTietChungTu_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "MaChungTu",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_BaoMatNguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatNganSach_BaoMatNguoiDung_MaNguoiSoHuu",
                schema: "BanGiamHieu",
                table: "DeXuatNganSach",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_BaoMatNguoiDung_MaNguoiDuyet",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DieuChinhNganSach_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "BanGiamHieu",
                table: "DieuChinhNganSach",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoSoTaiSanCoDinh_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "HoSoTaiSanCoDinh",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KyKeToan_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "KyKeToan",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhamViDonViNguoiDung_BaoMatNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhamViDonViNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhienDangNhap_BaoMatNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "PhienDangNhap",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuDangKyPhong_BaoMatNguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuDangKyPhong",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuMuonThietBi_BaoMatNguoiDung_MaNguoiSoHuu",
                schema: "GiangVien",
                table: "PhieuMuonThietBi",
                column: "MaNguoiSoHuu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_BaoMatNguoiDung_MaNguoiDuyet",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaNguoiDuyet",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThanhLyTaiSan_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "QuanLyTaiSan",
                table: "PhieuThanhLyTaiSan",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuTraThietBi_BaoMatNguoiDung_MaNguoiNghiemThu",
                schema: "GiangVien",
                table: "PhieuTraThietBi",
                column: "MaNguoiNghiemThu",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuyenBiTuChoiNguoiDung_BaoMatNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "QuyenBiTuChoiNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_ChiTietChungTu_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "MaChungTu",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrangThaiDocThongBao_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "QuanTriVien",
                table: "TrangThaiDocThongBao",
                column: "MaNguoiThucHien",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VaiTroNguoiDung_BaoMatNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "VaiTroNguoiDung",
                column: "MaNguoiDung",
                principalSchema: "QuanTriVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.Sql("""
                EXEC(N'CREATE PROCEDURE [QuanTriVien].[DongBoPhanQuyen] AS
                BEGIN
                SET NOCOUNT ON;
                DELETE FROM [QuanTriVien].[PhanQuyen];
                INSERT [QuanTriVien].[PhanQuyen] ([MaVaiTro],[TenVaiTro],[MaNguoiDung],[Quyen])
                SELECT DISTINCT r.[MaVaiTro],r.[TenVaiTro],ur.[MaNguoiDung],p.[Quyen]
                FROM [QuanTriVien].[VaiTroNguoiDung] ur
                JOIN [QuanTriVien].[VaiTro] r ON r.[MaVaiTro]=ur.[MaVaiTro]
                LEFT JOIN [QuanTriVien].[TuyChinhVaiTroNguoiDung] o ON o.[MaVaiTro]=ur.[MaVaiTro] AND o.[MaNguoiDung]=ur.[MaNguoiDung]
                CROSS APPLY (
                SELECT rp.[Quyen] FROM [QuanTriVien].[QuyenVaiTro] rp WHERE rp.[MaVaiTro]=ur.[MaVaiTro] AND o.[MaNguoiDung] IS NULL
                UNION ALL
                SELECT CONVERT(nvarchar(100),j.[value]) FROM OPENJSON(o.[DanhSachQuyenJson]) j WHERE o.[MaNguoiDung] IS NOT NULL
                ) p;
                END');
                """);
            migrationBuilder.Sql("""
                EXEC(N'CREATE TRIGGER [QuanTriVien].[TR_VaiTro_PhanQuyen] ON [QuanTriVien].[VaiTro] AFTER INSERT,UPDATE,DELETE AS BEGIN SET NOCOUNT ON; EXEC [QuanTriVien].[DongBoPhanQuyen]; END');
                """);
            migrationBuilder.Sql("""
                EXEC(N'CREATE TRIGGER [QuanTriVien].[TR_VaiTroNguoiDung_PhanQuyen] ON [QuanTriVien].[VaiTroNguoiDung] AFTER INSERT,UPDATE,DELETE AS BEGIN SET NOCOUNT ON; EXEC [QuanTriVien].[DongBoPhanQuyen]; END');
                """);
            migrationBuilder.Sql("""
                EXEC(N'CREATE TRIGGER [QuanTriVien].[TR_QuyenVaiTro_PhanQuyen] ON [QuanTriVien].[QuyenVaiTro] AFTER INSERT,UPDATE,DELETE AS BEGIN SET NOCOUNT ON; EXEC [QuanTriVien].[DongBoPhanQuyen]; END');
                """);
            migrationBuilder.Sql("""
                EXEC(N'CREATE TRIGGER [QuanTriVien].[TR_TuyChinhVaiTroNguoiDung_PhanQuyen] ON [QuanTriVien].[TuyChinhVaiTroNguoiDung] AFTER INSERT,UPDATE,DELETE AS BEGIN SET NOCOUNT ON; EXEC [QuanTriVien].[DongBoPhanQuyen]; END');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("THROW 51040, 'ExactFixSchema rollback requires a reviewed data-preserving migration.', 1;");
        }
    }
}
