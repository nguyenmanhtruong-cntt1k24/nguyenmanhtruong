using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FrontendFocusedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Stop before changing anything if features outside the current UI
            // have actual data. Review/export those accounts before migrating.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [QuanTriVien].[NguoiDung]
                    WHERE NULLIF([SoDienThoai], N'') IS NOT NULL
                       OR [DaXacNhanSoDienThoai] = 1
                       OR [DaXacNhanThuDienTu] = 1
                       OR [BatXacThucHaiYeuTo] = 1)
                    THROW 51010, 'Unused Identity fields contain data; review before simplifying.', 1;
                """);
            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_DonVi_TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropColumn(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropColumn(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropColumn(
                name: "BatXacThucHaiYeuTo",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DaXacNhanSoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DaXacNhanThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "TenDangNhapChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "ThuDienTuChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi");

            migrationBuilder.DropColumn(
                name: "SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                schema: "QuanTriVien",
                table: "DonVi",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "Ten",
                unique: true,
                filter: "[Ten] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true,
                filter: "[TenDangNhap] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                column: "ThuDienTu",
                unique: true,
                filter: "[ThuDienTu] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DonVi_Ten",
                schema: "QuanTriVien",
                table: "DonVi",
                column: "Ten",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                columns: new[] { "Nam", "SoChungTu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VaiTro_Ten",
                schema: "QuanTriVien",
                table: "VaiTro");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_TenDangNhap",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_NguoiDung_ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung");

            migrationBuilder.DropIndex(
                name: "IX_DonVi_Ten",
                schema: "QuanTriVien",
                table: "DonVi");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                schema: "QuanTriVien",
                table: "VaiTro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "VaiTro",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "VaiTro",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "TenDangNhap",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AddColumn<bool>(
                name: "BatXacThucHaiYeuTo",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DaXacNhanSoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DaXacNhanThuDienTu",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DauDongThoi",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenDangNhapChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThuDienTuChuanHoa",
                schema: "QuanTriVien",
                table: "NguoiDung",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ten",
                schema: "QuanTriVien",
                table: "DonVi",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SoChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AddColumn<string>(
                name: "SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Rebuild derived values before reintroducing unique indexes.
            migrationBuilder.Sql("""
                UPDATE [QuanTriVien].[NguoiDung]
                    SET [TenDangNhapChuanHoa] = UPPER([TenDangNhap]),
                        [ThuDienTuChuanHoa] = UPPER([ThuDienTu]),
                        [DauDongThoi] = CONVERT(nvarchar(36), NEWID());
                UPDATE [QuanTriVien].[VaiTro]
                    SET [TenChuanHoa] = UPPER([Ten]),
                        [DauDongThoi] = CONVERT(nvarchar(36), NEWID());
                UPDATE [QuanTriVien].[DonVi] SET [TenChuanHoa] = UPPER([Ten]);
                UPDATE [KeToan].[ChungTuTaiChinh] SET [SoChungTuChuanHoa] = UPPER([SoChungTu]);
                """);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "QuanTriVien",
                table: "VaiTro",
                column: "TenChuanHoa",
                unique: true,
                filter: "[TenChuanHoa] IS NOT NULL");

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
                name: "IX_DonVi_TenChuanHoa",
                schema: "QuanTriVien",
                table: "DonVi",
                column: "TenChuanHoa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChungTuTaiChinh_Nam_SoChungTuChuanHoa",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                columns: new[] { "Nam", "SoChungTuChuanHoa" },
                unique: true);
        }
    }
}
