using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoomUseLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomUseLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    ActualStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Attendees = table.Column<int>(type: "int", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Issue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomUseLogs", x => x.Id);
                    table.CheckConstraint("CK_RoomLog_Actual", "([Used] = 0 AND [ActualStart] IS NULL AND [ActualEnd] IS NULL AND [Attendees] IS NULL) OR ([Used] = 1 AND [ActualStart] IS NOT NULL AND [ActualEnd] IS NOT NULL AND [Attendees] IS NOT NULL AND [ActualStart] < [ActualEnd] AND [Attendees] >= 0)");
                    table.ForeignKey(
                        name: "FK_RoomUseLogs_RoomRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RoomRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomUseLogs_RequestId",
                table: "RoomUseLogs",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomUseLogs");
        }
    }
}
