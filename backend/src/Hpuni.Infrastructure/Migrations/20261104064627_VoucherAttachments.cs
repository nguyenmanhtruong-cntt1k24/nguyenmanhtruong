using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpuni.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VoucherAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoucherAttachments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAttachments", x => x.Id);
                    table.CheckConstraint("CK_Attachment_Size", "[Size] BETWEEN 1 AND 2097152");
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_FinanceEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "FinanceEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoucherAttachmentData",
                columns: table => new
                {
                    AttachmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAttachmentData", x => x.AttachmentId);
                    table.CheckConstraint("CK_AttachmentData_Size", "DATALENGTH([Content]) BETWEEN 1 AND 2097152");
                    table.ForeignKey(
                        name: "FK_VoucherAttachmentData_VoucherAttachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "VoucherAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_ActorId",
                table: "VoucherAttachments",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_EntryId",
                table: "VoucherAttachments",
                column: "EntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherAttachmentData");

            migrationBuilder.DropTable(
                name: "VoucherAttachments");
        }
    }
}
