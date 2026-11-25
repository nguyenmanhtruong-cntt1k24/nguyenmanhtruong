using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteFrontendSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Refuse to discard data from optional Identity features, or conflicting
            // allocation ownership. All checks precede the first schema change.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [QuanTriVien].[DangNhapNgoai])
                   OR EXISTS (SELECT 1 FROM [QuanTriVien].[MaXacThucNguoiDung])
                   OR EXISTS (SELECT 1 FROM [QuanTriVien].[ThuocTinhNguoiDung])
                   OR EXISTS (SELECT 1 FROM [QuanTriVien].[ThuocTinhVaiTro])
                    THROW 51020, 'Optional Identity tables contain data; review before simplifying.', 1;
                IF EXISTS (
                    SELECT 1 FROM [BanGiamHieu].[PhanBoNganSach] a
                    LEFT JOIN [BanGiamHieu].[DeXuatNganSach] p ON p.[Ma] = a.[MaDeXuat]
                    WHERE p.[Ma] IS NULL OR a.[MaDonVi] <> p.[MaDonVi]
                       OR a.[Nam] <> p.[Nam] OR a.[DanhMuc] <> p.[DanhMuc])
                    THROW 51021, 'Allocation metadata differs from proposal; review before simplifying.', 1;
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_PhanBoNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropTable(
                name: "DangNhapNgoai",
                schema: "QuanTriVien");

            migrationBuilder.DropTable(
                name: "MaXacThucNguoiDung",
                schema: "QuanTriVien");

            migrationBuilder.DropTable(
                name: "ThuocTinhNguoiDung",
                schema: "QuanTriVien");

            migrationBuilder.DropTable(
                name: "ThuocTinhVaiTro",
                schema: "QuanTriVien");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanBoNganSach",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropIndex(
                name: "IX_PhanBoNganSach_MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropIndex(
                name: "IX_PhanBoNganSach_MaDonVi_Nam_DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropColumn(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropColumn(
                name: "DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropColumn(
                name: "MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.DropColumn(
                name: "Nam",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanBoNganSach",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "MaDeXuat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PhanBoNganSach",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach");

            migrationBuilder.AddColumn<string>(
                name: "Ma",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Nam",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE a SET a.[Ma] = CONVERT(nvarchar(36), NEWID()),
                    a.[MaDonVi] = p.[MaDonVi], a.[Nam] = p.[Nam], a.[DanhMuc] = p.[DanhMuc]
                FROM [BanGiamHieu].[PhanBoNganSach] a
                JOIN [BanGiamHieu].[DeXuatNganSach] p ON p.[Ma] = a.[MaDeXuat];
                """);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhanBoNganSach",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "Ma");

            migrationBuilder.CreateTable(
                name: "DangNhapNgoai",
                schema: "QuanTriVien",
                columns: table => new
                {
                    NhaCungCapDangNhap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KhoaNhaCungCap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenHienThiNhaCungCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangNhapNgoai", x => new { x.NhaCungCapDangNhap, x.KhoaNhaCungCap });
                    table.ForeignKey(
                        name: "FK_DangNhapNgoai_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalSchema: "QuanTriVien",
                        principalTable: "NguoiDung",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaXacThucNguoiDung",
                schema: "QuanTriVien",
                columns: table => new
                {
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NhaCungCapDangNhap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaXacThucNguoiDung", x => new { x.MaNguoiDung, x.NhaCungCapDangNhap, x.Ten });
                    table.ForeignKey(
                        name: "FK_MaXacThucNguoiDung_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalSchema: "QuanTriVien",
                        principalTable: "NguoiDung",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThuocTinhNguoiDung",
                schema: "QuanTriVien",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiThuocTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriThuocTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNguoiDung = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuocTinhNguoiDung", x => x.Ma);
                    table.ForeignKey(
                        name: "FK_ThuocTinhNguoiDung_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalSchema: "QuanTriVien",
                        principalTable: "NguoiDung",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThuocTinhVaiTro",
                schema: "QuanTriVien",
                columns: table => new
                {
                    Ma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiThuocTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTriThuocTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaVaiTro = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuocTinhVaiTro", x => x.Ma);
                    table.ForeignKey(
                        name: "FK_ThuocTinhVaiTro_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalSchema: "QuanTriVien",
                        principalTable: "VaiTro",
                        principalColumn: "Ma",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhanBoNganSach_MaDeXuat",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "MaDeXuat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhanBoNganSach_MaDonVi_Nam_DanhMuc",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                columns: new[] { "MaDonVi", "Nam", "DanhMuc" });

            migrationBuilder.CreateIndex(
                name: "IX_DangNhapNgoai_MaNguoiDung",
                schema: "QuanTriVien",
                table: "DangNhapNgoai",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThuocTinhNguoiDung_MaNguoiDung",
                schema: "QuanTriVien",
                table: "ThuocTinhNguoiDung",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThuocTinhVaiTro_MaVaiTro",
                schema: "QuanTriVien",
                table: "ThuocTinhVaiTro",
                column: "MaVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_PhanBoNganSach_DonVi_MaDonVi",
                schema: "BanGiamHieu",
                table: "PhanBoNganSach",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "Ma",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
