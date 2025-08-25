using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutCom.Migrations
{
    /// <inheritdoc />
    public partial class AddFileManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentFolderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileItems_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FileItems_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileItems_FileItems_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "FileItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "FileShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileItemId = table.Column<int>(type: "int", nullable: false),
                    SharedWithUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SharedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileShares_AspNetUsers_SharedByUserId",
                        column: x => x.SharedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileShares_AspNetUsers_SharedWithUserId",
                        column: x => x.SharedWithUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileShares_FileItems_FileItemId",
                        column: x => x.FileItemId,
                        principalTable: "FileItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 15, 55, 19, 116, DateTimeKind.Utc).AddTicks(2778));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 15, 55, 19, 116, DateTimeKind.Utc).AddTicks(2781));

            migrationBuilder.CreateIndex(
                name: "IX_FileItems_ClientId",
                table: "FileItems",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FileItems_OwnerId",
                table: "FileItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FileItems_ParentFolderId",
                table: "FileItems",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileItems_Path",
                table: "FileItems",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_FileItems_Type",
                table: "FileItems",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_FileItemId",
                table: "FileShares",
                column: "FileItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_SharedByUserId",
                table: "FileShares",
                column: "SharedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_SharedWithUserId",
                table: "FileShares",
                column: "SharedWithUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileShares");

            migrationBuilder.DropTable(
                name: "FileItems");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 21, 21, 31, 44, 367, DateTimeKind.Utc).AddTicks(2586));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 21, 21, 31, 44, 367, DateTimeKind.Utc).AddTicks(2589));
        }
    }
}
