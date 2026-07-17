using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInTransitAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InTransitAt",
                table: "DeliveryAssignments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 17, 17, 40, 53, 845, DateTimeKind.Utc).AddTicks(1391));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 17, 17, 40, 53, 845, DateTimeKind.Utc).AddTicks(3280));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 17, 17, 40, 53, 845, DateTimeKind.Utc).AddTicks(4946));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InTransitAt",
                table: "DeliveryAssignments");

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 17, 16, 48, 59, 271, DateTimeKind.Utc).AddTicks(3019));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 17, 16, 48, 59, 271, DateTimeKind.Utc).AddTicks(4318));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 17, 16, 48, 59, 271, DateTimeKind.Utc).AddTicks(5507));
        }
    }
}
