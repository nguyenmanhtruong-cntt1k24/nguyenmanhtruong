using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExactFinanceDependencyMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE [KeToan].[ChiTietChungTu] DROP CONSTRAINT [FK_ChiTietChungTu_DonVi];
                ALTER TABLE [KeToan].[ChiTietChungTu] DROP CONSTRAINT [FK_ChiTietChungTu_DeNghiThanhToan];
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_ButToanDao_ChiTietChungTu_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietChungTu_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "ChiTietChungTu");

            migrationBuilder.DropForeignKey(
                name: "FK_ChungTuTaiChinh_BaoMatNguoiDung_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropForeignKey(
                name: "FK_TepDinhKemChungTu_ChiTietChungTu_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu");

            migrationBuilder.DropIndex(
                name: "IX_ChungTuTaiChinh_MaNguoiThucHien",
                schema: "KeToan",
                table: "ChungTuTaiChinh");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Entry_Amount",
                schema: "KeToan",
                table: "ChungTuTaiChinh");


            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChungTu_MaDonVi_Nam_Ngay_MaChungTu",
                schema: "KeToan",
                table: "ChiTietChungTu",
                columns: new[] { "MaDonVi", "Nam", "Ngay", "MaChungTu" });

            migrationBuilder.AddForeignKey(
                name: "FK_ButToanDao_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "ButToanDao",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "MaChungTu",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietChungTu_DeNghiThanhToan_MaDeNghiThanhToan",
                schema: "KeToan",
                table: "ChiTietChungTu",
                column: "MaDeNghiThanhToan",
                principalSchema: "KeToan",
                principalTable: "DeNghiThanhToan",
                principalColumn: "MaDeNghiThanhToan",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietChungTu_DonVi_MaDonVi",
                schema: "KeToan",
                table: "ChiTietChungTu",
                column: "MaDonVi",
                principalSchema: "QuanTriVien",
                principalTable: "DonVi",
                principalColumn: "MaDonVi",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChungTuTaiChinh_ChiTietChungTu_MaChungTu",
                schema: "KeToan",
                table: "ChungTuTaiChinh",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChiTietChungTu",
                principalColumn: "MaChungTu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TepDinhKemChungTu_ChungTuTaiChinh_MaChungTu",
                schema: "KeToan",
                table: "TepDinhKemChungTu",
                column: "MaChungTu",
                principalSchema: "KeToan",
                principalTable: "ChungTuTaiChinh",
                principalColumn: "MaChungTu",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.Sql("""
                ALTER TABLE [KeToan].[ChungTuTaiChinh] ADD CONSTRAINT [FK_ChungTuTaiChinh_NguoiDung_MaNguoiThucHien] FOREIGN KEY ([MaNguoiThucHien]) REFERENCES [QuanTriVien].[NguoiDung]([MaNguoiDung]);
                ALTER TABLE [KeToan].[ChungTuTaiChinh] ADD CONSTRAINT [CK_Entry_Amount] CHECK ([SoTien]>0 AND [PhanLoai] IN ('receipt','expense'));
                CREATE INDEX [IX_ChungTuTaiChinh_MaNguoiThucHien] ON [KeToan].[ChungTuTaiChinh]([MaNguoiThucHien]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("THROW 51042, 'Exact finance rollback requires a reviewed migration.', 1;");
        }
    }
}
