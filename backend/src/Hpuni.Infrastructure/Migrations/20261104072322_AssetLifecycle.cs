using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AssetLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Movement_Quantity",
                table: "EquipmentMovements");

            migrationBuilder.CreateTable(
                name: "AssetDisposals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false),
                    Damaged = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDisposals", x => x.Id);
                    table.CheckConstraint("CK_AssetDisposal_Quantity", "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Good]+[Damaged] BETWEEN 1 AND 1000000");
                    table.CheckConstraint("CK_AssetDisposal_Status", "([Status] = 'pending' AND [ReviewerId] IS NULL AND [ReviewedAt] IS NULL) OR ([Status] IN ('approved','rejected') AND [ReviewerId] IS NOT NULL AND [ReviewedAt] IS NOT NULL AND [ReviewerId] <> [ActorId])");
                    table.ForeignKey(
                        name: "FK_AssetDisposals_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetDisposals_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetDisposals_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetDisposals_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FixedAssetProfiles",
                columns: table => new
                {
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AcquisitionValue = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    AcquisitionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetProfiles", x => x.AssetGroupId);
                    table.CheckConstraint("CK_FixedAssetProfile_Value", "[AcquisitionValue] BETWEEN 1 AND 1000000000000 AND [AcquisitionDate] >= '1900-01-01'");
                    table.ForeignKey(
                        name: "FK_FixedAssetProfiles_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetProfiles_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movement_Quantity",
                table: "EquipmentMovements",
                sql: "([Kind] IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN -1000000 AND -1) OR ([Kind] NOT IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN 1 AND 1000000)");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_ActorId",
                table: "AssetDisposals",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_AssetGroupId_Status",
                table: "AssetDisposals",
                columns: new[] { "AssetGroupId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_DepartmentId_Status_CreatedAt",
                table: "AssetDisposals",
                columns: new[] { "DepartmentId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDisposals_ReviewerId",
                table: "AssetDisposals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetProfiles_ActorId",
                table: "FixedAssetProfiles",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDisposals");

            migrationBuilder.DropTable(
                name: "FixedAssetProfiles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movement_Quantity",
                table: "EquipmentMovements");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movement_Quantity",
                table: "EquipmentMovements",
                sql: "[Quantity] BETWEEN 1 AND 1000000");
        }
    }
}
