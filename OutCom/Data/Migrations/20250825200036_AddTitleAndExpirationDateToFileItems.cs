using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutCom.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleAndExpirationDateToFileItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "FileItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "FileItems",
                type: "nvarchar(255)",
                maxLength: 255,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "FileItems");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "FileItems");

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
        }
    }
}
