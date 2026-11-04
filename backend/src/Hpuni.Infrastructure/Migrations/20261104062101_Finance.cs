using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Finance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetProposals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    LinesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetProposals", x => x.Id);
                    table.CheckConstraint("CK_Proposal_Amount", "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_BudgetProposals_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetProposals_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetProposals_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MaintenanceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.Id);
                    table.CheckConstraint("CK_Payment_Amount", "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100");
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_MaintenanceOrders_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "MaintenanceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAllocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProposalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAllocations", x => x.Id);
                    table.CheckConstraint("CK_Allocation_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_BudgetAllocations_BudgetProposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "BudgetProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetAllocations_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinanceEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    Voucher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NormalizedVoucher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentRequestId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceEntries", x => x.Id);
                    table.CheckConstraint("CK_Entry_Amount", "[Amount] > 0 AND [Year] BETWEEN 2000 AND 2100 AND YEAR([Date]) = [Year] AND [Kind] IN ('receipt','expense')");
                    table.ForeignKey(
                        name: "FK_FinanceEntries_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinanceEntries_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinanceEntries_PaymentRequests_PaymentRequestId",
                        column: x => x.PaymentRequestId,
                        principalTable: "PaymentRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllocations_DepartmentId_Year_Category",
                table: "BudgetAllocations",
                columns: new[] { "DepartmentId", "Year", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAllocations_ProposalId",
                table: "BudgetAllocations",
                column: "ProposalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetProposals_DepartmentId",
                table: "BudgetProposals",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetProposals_OwnerId",
                table: "BudgetProposals",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetProposals_ReviewerId",
                table: "BudgetProposals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_ActorId",
                table: "FinanceEntries",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_DepartmentId_Year_Category_Kind",
                table: "FinanceEntries",
                columns: new[] { "DepartmentId", "Year", "Category", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_PaymentRequestId",
                table: "FinanceEntries",
                column: "PaymentRequestId",
                unique: true,
                filter: "[PaymentRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceEntries_Year_NormalizedVoucher",
                table: "FinanceEntries",
                columns: new[] { "Year", "NormalizedVoucher" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_DepartmentId",
                table: "PaymentRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_MaintenanceId",
                table: "PaymentRequests",
                column: "MaintenanceId",
                unique: true,
                filter: "[MaintenanceId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAllocations");

            migrationBuilder.DropTable(
                name: "FinanceEntries");

            migrationBuilder.DropTable(
                name: "BudgetProposals");

            migrationBuilder.DropTable(
                name: "PaymentRequests");
        }
    }
}
