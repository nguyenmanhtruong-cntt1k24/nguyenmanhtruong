using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EquipmentLending : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetGroups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Damaged = table.Column<int>(type: "int", nullable: false),
                    Lost = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetGroups", x => x.Id);
                    table.CheckConstraint("CK_Asset_Quantity", "[Total] BETWEEN 0 AND 1000000 AND [Damaged] >= 0 AND [Lost] >= 0 AND [Damaged] + [Lost] <= [Total]");
                    table.ForeignKey(
                        name: "FK_AssetGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentLoans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Returned = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HandedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReportedReceived = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentLoans", x => x.Id);
                    table.CheckConstraint("CK_Loan_Interval", "[Start] < [End]");
                    table.CheckConstraint("CK_Loan_Quantity", "[Quantity] BETWEEN 1 AND 1000000 AND [Returned] >= 0 AND [Returned] <= [Quantity] AND ([ReportedReceived] IS NULL OR [ReportedReceived] BETWEEN 0 AND [Quantity])");
                    table.ForeignKey(
                        name: "FK_EquipmentLoans_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentLoans_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentLoans_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMovements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Kind = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    At = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMovements", x => x.Id);
                    table.CheckConstraint("CK_Movement_Quantity", "[Quantity] BETWEEN 1 AND 1000000");
                    table.ForeignKey(
                        name: "FK_EquipmentMovements_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentMovements_AssetGroups_AssetGroupId",
                        column: x => x.AssetGroupId,
                        principalTable: "AssetGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentMovements_EquipmentLoans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "EquipmentLoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentReturns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false),
                    Damaged = table.Column<int>(type: "int", nullable: false),
                    Lost = table.Column<int>(type: "int", nullable: false),
                    AcceptedGood = table.Column<int>(type: "int", nullable: true),
                    AcceptedDamaged = table.Column<int>(type: "int", nullable: true),
                    AcceptedLost = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AcceptanceNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AcceptedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    At = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentReturns", x => x.Id);
                    table.CheckConstraint("CK_Return_Quantity", "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Good]+[Damaged]+[Lost] BETWEEN 1 AND 1000000 AND (([AcceptedAt] IS NULL AND [AcceptedGood] IS NULL AND [AcceptedDamaged] IS NULL AND [AcceptedLost] IS NULL AND [AcceptedBy] IS NULL) OR ([AcceptedAt] IS NOT NULL AND [AcceptedBy] IS NOT NULL AND [AcceptedGood] >= 0 AND [AcceptedDamaged] >= 0 AND [AcceptedLost] >= 0 AND [AcceptedGood]+[AcceptedDamaged]+[AcceptedLost] BETWEEN 1 AND [Good]+[Damaged]+[Lost]))");
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_AspNetUsers_AcceptedBy",
                        column: x => x.AcceptedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentReturns_EquipmentLoans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "EquipmentLoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroups_Code",
                table: "AssetGroups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroups_DepartmentId",
                table: "AssetGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentLoans_AssetGroupId_Status_Start_End",
                table: "EquipmentLoans",
                columns: new[] { "AssetGroupId", "Status", "Start", "End" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentLoans_DepartmentId",
                table: "EquipmentLoans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentLoans_OwnerId_CreatedAt",
                table: "EquipmentLoans",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMovements_ActorId",
                table: "EquipmentMovements",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMovements_AssetGroupId_At",
                table: "EquipmentMovements",
                columns: new[] { "AssetGroupId", "At" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMovements_LoanId",
                table: "EquipmentMovements",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_AcceptedBy",
                table: "EquipmentReturns",
                column: "AcceptedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentReturns_LoanId",
                table: "EquipmentReturns",
                column: "LoanId",
                unique: true,
                filter: "[AcceptedAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentMovements");

            migrationBuilder.DropTable(
                name: "EquipmentReturns");

            migrationBuilder.DropTable(
                name: "EquipmentLoans");

            migrationBuilder.DropTable(
                name: "AssetGroups");
        }
    }
}
