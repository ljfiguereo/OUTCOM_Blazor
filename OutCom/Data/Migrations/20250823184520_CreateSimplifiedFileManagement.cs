using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutCom.Migrations
{
    /// <inheritdoc />
    public partial class CreateSimplifiedFileManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileItems_AspNetUsers_ClientId",
                table: "FileItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FileItems_FileItems_ParentFolderId",
                table: "FileItems");

            migrationBuilder.DropForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedByUserId",
                table: "FileShares");

            migrationBuilder.DropForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedWithUserId",
                table: "FileShares");

            migrationBuilder.DropIndex(
                name: "IX_FileItems_ClientId",
                table: "FileItems");

            migrationBuilder.DropIndex(
                name: "IX_FileItems_ParentFolderId",
                table: "FileItems");

            migrationBuilder.DropIndex(
                name: "IX_FileItems_Path",
                table: "FileItems");

            migrationBuilder.DropIndex(
                name: "IX_FileItems_Type",
                table: "FileItems");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "FileItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 18, 45, 15, 262, DateTimeKind.Utc).AddTicks(4950));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 18, 45, 15, 262, DateTimeKind.Utc).AddTicks(4962));

            migrationBuilder.AddForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedByUserId",
                table: "FileShares",
                column: "SharedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedWithUserId",
                table: "FileShares",
                column: "SharedWithUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedByUserId",
                table: "FileShares");

            migrationBuilder.DropForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedWithUserId",
                table: "FileShares");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "FileItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_FileItems_AspNetUsers_ClientId",
                table: "FileItems",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FileItems_FileItems_ParentFolderId",
                table: "FileItems",
                column: "ParentFolderId",
                principalTable: "FileItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedByUserId",
                table: "FileShares",
                column: "SharedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedWithUserId",
                table: "FileShares",
                column: "SharedWithUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
