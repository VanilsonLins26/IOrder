using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPartnerAndOutForDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryPartner",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "FreeDeliveryRadiusKm",
                table: "Stores",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "RequestedEarlyDelivery",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "DeliveryPartner", "FreeDeliveryRadiusKm" },
                values: new object[] { 0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "DeliveryPartner", "FreeDeliveryRadiusKm" },
                values: new object[] { 0, 0.0 });

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                columns: new[] { "DeliveryPartner", "FreeDeliveryRadiusKm" },
                values: new object[] { 0, 0.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryPartner",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "FreeDeliveryRadiusKm",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "RequestedEarlyDelivery",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "ExpiresAt",
                value: new DateTime(2027, 1, 16, 17, 20, 32, 206, DateTimeKind.Utc).AddTicks(6296));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "ExpiresAt",
                value: new DateTime(2026, 10, 16, 17, 20, 32, 206, DateTimeKind.Utc).AddTicks(7599));

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"),
                column: "ExpiresAt",
                value: new DateTime(2027, 7, 16, 17, 20, 32, 206, DateTimeKind.Utc).AddTicks(9060));
        }
    }
}
