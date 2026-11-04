using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Return_Quantity",
                table: "EquipmentReturns");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Asset_Quantity",
                table: "AssetGroups");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Return_Quantity",
                table: "EquipmentReturns",
                sql: "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Good]+[Damaged]+[Lost] BETWEEN 1 AND 1000000 AND (([AcceptedAt] IS NULL AND [AcceptedGood] IS NULL AND [AcceptedDamaged] IS NULL AND [AcceptedLost] IS NULL AND [AcceptedBy] IS NULL) OR ([AcceptedAt] IS NOT NULL AND [AcceptedBy] IS NOT NULL AND [AcceptedGood] IS NOT NULL AND [AcceptedDamaged] IS NOT NULL AND [AcceptedLost] IS NOT NULL AND [AcceptedGood] BETWEEN 0 AND 1000000 AND [AcceptedDamaged] BETWEEN 0 AND 1000000 AND [AcceptedLost] BETWEEN 0 AND 1000000 AND [AcceptedGood]+[AcceptedDamaged]+[AcceptedLost] BETWEEN 1 AND [Good]+[Damaged]+[Lost]))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_Status",
                table: "EquipmentLoans",
                sql: "[Status] IN ('draft','pending','approved','rejected','cancelled','ready','receipt_disputed','borrowed','return_pending','completed') AND (([Status] IN ('ready','receipt_disputed','borrowed','return_pending','completed') AND [HandedAt] IS NOT NULL) OR ([Status] IN ('draft','pending','approved','rejected','cancelled') AND [HandedAt] IS NULL))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Asset_Quantity",
                table: "AssetGroups",
                sql: "[Total] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Damaged] + [Lost] <= [Total]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Return_Quantity",
                table: "EquipmentReturns");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_Status",
                table: "EquipmentLoans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Asset_Quantity",
                table: "AssetGroups");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Return_Quantity",
                table: "EquipmentReturns",
                sql: "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Good]+[Damaged]+[Lost] BETWEEN 1 AND 1000000 AND (([AcceptedAt] IS NULL AND [AcceptedGood] IS NULL AND [AcceptedDamaged] IS NULL AND [AcceptedLost] IS NULL AND [AcceptedBy] IS NULL) OR ([AcceptedAt] IS NOT NULL AND [AcceptedBy] IS NOT NULL AND [AcceptedGood] >= 0 AND [AcceptedDamaged] >= 0 AND [AcceptedLost] >= 0 AND [AcceptedGood]+[AcceptedDamaged]+[AcceptedLost] BETWEEN 1 AND [Good]+[Damaged]+[Lost]))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Asset_Quantity",
                table: "AssetGroups",
                sql: "[Total] BETWEEN 0 AND 1000000 AND [Damaged] >= 0 AND [Lost] >= 0 AND [Damaged] + [Lost] <= [Total]");
        }
    }
}
