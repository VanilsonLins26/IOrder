using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscountType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscountValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MinPurchaseAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentUsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Active", "Code", "DiscountType", "DiscountValue", "ExpiresAt", "MaxDiscountAmount", "MaxUsageCount", "MinPurchaseAmount" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), true, "BEMVINDO10", "Percentage", 10m, new DateTime(2027, 1, 6, 21, 13, 53, 944, DateTimeKind.Utc).AddTicks(1844), 30m, 100, 50m },
                    { new Guid("50000000-0000-0000-0000-000000000002"), true, "FRETE20", "FixedAmount", 20m, new DateTime(2026, 10, 6, 21, 13, 53, 944, DateTimeKind.Utc).AddTicks(3115), null, 50, 80m },
                    { new Guid("50000000-0000-0000-0000-000000000003"), true, "NIVER15", "Percentage", 15m, new DateTime(2027, 7, 6, 21, 13, 53, 944, DateTimeKind.Utc).AddTicks(4176), 50m, 200, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
