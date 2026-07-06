using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyProductName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "OpeningHours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "OpeningHours",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);
        }
    }
}
