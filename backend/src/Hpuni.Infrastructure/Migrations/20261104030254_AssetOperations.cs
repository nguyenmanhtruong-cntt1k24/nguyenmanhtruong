using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AssetOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipmentUseLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    ActualStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Issue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentUseLogs", x => x.Id);
                    table.CheckConstraint("CK_EquipmentLog_Valid", "[Status] IN ('submitted','needs_changes','confirmed') AND (([Used]=0 AND [ActualStart] IS NULL AND [ActualEnd] IS NULL AND [Quantity] IS NULL) OR ([Used]=1 AND [ActualStart] IS NOT NULL AND [ActualEnd] IS NOT NULL AND [Quantity] IS NOT NULL AND [ActualStart]<[ActualEnd] AND [Quantity] BETWEEN 1 AND 1000000))");
                    table.ForeignKey(
                        name: "FK_EquipmentUseLogs_EquipmentLoans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "EquipmentLoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventorySessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    At = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySessions", x => x.Id);
                    table.CheckConstraint("CK_Inventory_Status", "[Status] IN ('open','completed')");
                    table.ForeignKey(
                        name: "FK_InventorySessions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceOrders", x => x.Id);
                    table.CheckConstraint("CK_Maintenance_Valid", "[Quantity] BETWEEN 1 AND 1000000 AND [Start] < [End] AND [Mode] IN ('service','repair') AND [Status] IN ('planned','running','completed','cancelled') AND [Cost] BETWEEN 0 AND 1000000000000 AND (([Status]='completed' AND [Result] IN ('good','unavailable')) OR ([Status]<>'completed' AND [Result]=''))");
                    table.ForeignKey(
                        name: "FK_MaintenanceOrders_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceOrders_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Expected = table.Column<int>(type: "int", nullable: false),
                    ExpectedDamaged = table.Column<int>(type: "int", nullable: false),
                    ExpectedLost = table.Column<int>(type: "int", nullable: false),
                    AssetVersion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Actual = table.Column<int>(type: "int", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => new { x.SessionId, x.AssetGroupId });
                    table.CheckConstraint("CK_Inventory_Count", "[Expected] BETWEEN 0 AND 1000000 AND [ExpectedDamaged]>=0 AND [ExpectedLost]>=0 AND [ExpectedDamaged]+[ExpectedLost]<=[Expected] AND ([Actual] IS NULL OR [Actual] BETWEEN 0 AND 1000000) AND [Condition] IN ('','good','damaged','missing')");
                    table.ForeignKey(
                        name: "FK_InventoryItems_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItems_InventorySessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "InventorySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentUseLogs_LoanId",
                table: "EquipmentUseLogs",
                column: "LoanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_AssetGroupId",
                table: "InventoryItems",
                column: "AssetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySessions_DepartmentId",
                table: "InventorySessions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_AssetGroupId_Status",
                table: "MaintenanceOrders",
                columns: new[] { "AssetGroupId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceOrders_DepartmentId",
                table: "MaintenanceOrders",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentUseLogs");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MaintenanceOrders");

            migrationBuilder.DropTable(
                name: "InventorySessions");
        }
    }
}
