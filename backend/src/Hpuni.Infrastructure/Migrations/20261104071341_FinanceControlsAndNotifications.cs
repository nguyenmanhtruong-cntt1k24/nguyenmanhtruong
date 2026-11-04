using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinanceControlsAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "FinanceEntries");

            migrationBuilder.CreateTable(
                name: "BudgetAdjustments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAdjustments", x => x.Id);
                    table.CheckConstraint("CK_Adjustment_Amount", "[Amount] <> 0 AND ABS([Amount]) <= 1000000000000 AND [Year] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_BudgetAdjustments_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAdjustments_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAdjustments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceReversals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceReversals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceReversals_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinanceReversals_FinanceEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "FinanceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalPeriods",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalPeriods", x => new { x.DepartmentId, x.Year });
                    table.CheckConstraint("CK_FiscalPeriod_Year", "[Year] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_FiscalPeriods_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FiscalPeriods_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationReadStates",
                columns: table => new
                {
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SourceVersion = table.Column<byte[]>(type: "varbinary(8)", maxLength: 8, nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationReadStates", x => new { x.ActorId, x.SourceKey });
                    table.ForeignKey(
                        name: "FK_NotificationReadStates_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "FinanceEntries",
                column: "PaymentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAdjustments_ActorId",
                table: "BudgetAdjustments",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAdjustments_DepartmentId_Year_Category_Status",
                table: "BudgetAdjustments",
                columns: new[] { "DepartmentId", "Year", "Category", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAdjustments_ReviewerId",
                table: "BudgetAdjustments",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceReversals_ActorId",
                table: "FinanceReversals",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceReversals_EntryId",
                table: "FinanceReversals",
                column: "EntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiscalPeriods_ActorId",
                table: "FiscalPeriods",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAdjustments");

            migrationBuilder.DropTable(
                name: "FinanceReversals");

            migrationBuilder.DropTable(
                name: "FiscalPeriods");

            migrationBuilder.DropTable(
                name: "NotificationReadStates");

            migrationBuilder.DropIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "FinanceEntries");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "FinanceEntries",
                column: "PaymentRequestId",
                unique: true,
                filter: "[PaymentRequestId] IS NOT NULL");
        }
    }
}
