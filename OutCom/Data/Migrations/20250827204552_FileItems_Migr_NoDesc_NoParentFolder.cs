using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutCom.Migrations
{
    /// <inheritdoc />
    public partial class FileItems_Migr_NoDesc_NoParentFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FileItems");

            migrationBuilder.DropColumn(
                name: "ParentFolderId",
                table: "FileItems");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 27, 20, 45, 51, 530, DateTimeKind.Utc).AddTicks(8496));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 27, 20, 45, 51, 530, DateTimeKind.Utc).AddTicks(8498));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FileItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentFolderId",
                table: "FileItems",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 25, 20, 0, 35, 941, DateTimeKind.Utc).AddTicks(8815));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 25, 20, 0, 35, 941, DateTimeKind.Utc).AddTicks(8818));
        }
    }
}
