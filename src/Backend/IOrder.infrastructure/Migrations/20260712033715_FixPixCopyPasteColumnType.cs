using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPixCopyPasteColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PixCopyPaste",
                table: "Payments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 12, 3, 37, 13, 982, DateTimeKind.Utc).AddTicks(1762));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 12, 3, 37, 13, 982, DateTimeKind.Utc).AddTicks(3017));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 12, 3, 37, 13, 982, DateTimeKind.Utc).AddTicks(4125));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PixCopyPaste",
                table: "Payments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 12, 0, 15, 34, 248, DateTimeKind.Utc).AddTicks(5303));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 12, 0, 15, 34, 248, DateTimeKind.Utc).AddTicks(6701));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 12, 0, 15, 34, 248, DateTimeKind.Utc).AddTicks(7948));
        }
    }
}
