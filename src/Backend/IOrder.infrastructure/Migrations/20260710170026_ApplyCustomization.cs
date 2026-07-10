using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyCustomization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 10, 17, 0, 24, 630, DateTimeKind.Utc).AddTicks(2512));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 10, 17, 0, 24, 630, DateTimeKind.Utc).AddTicks(4491));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 10, 17, 0, 24, 630, DateTimeKind.Utc).AddTicks(6175));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 10, 15, 29, 49, 696, DateTimeKind.Utc).AddTicks(6116));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 10, 15, 29, 49, 696, DateTimeKind.Utc).AddTicks(8034));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 10, 15, 29, 49, 696, DateTimeKind.Utc).AddTicks(9852));
        }
    }
}
