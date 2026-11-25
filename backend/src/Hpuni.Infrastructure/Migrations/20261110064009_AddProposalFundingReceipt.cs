using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalFundingReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietChungTu_MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu",
                column: "MaDuToanNhanKinhPhi");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietChungTu_DeXuatNganSach_MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu",
                column: "MaDuToanNhanKinhPhi",
                principalSchema: "BanGiamHieu",
                principalTable: "DeXuatNganSach",
                principalColumn: "MaDeXuat",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietChungTu_DeXuatNganSach_MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietChungTu_MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu");

            migrationBuilder.DropColumn(
                name: "MaDuToanNhanKinhPhi",
                schema: "KeToan",
                table: "ChiTietChungTu");
        }
    }
}
