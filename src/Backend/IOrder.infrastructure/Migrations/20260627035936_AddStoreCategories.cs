using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Products",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Position = table.Column<int>(type: "int", nullable: false),
                    StoreId1 = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Stores_StoreId1",
                        column: x => x.StoreId1,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StoreCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "StoreCategories",
                columns: new[] { "Id", "Active", "IconUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("1dee1a76-b7d5-429c-9250-70a735dcc297"), true, "", "Japonês" },
                    { new Guid("1e88ad1a-c9e8-4d86-8e0b-3da38d55650e"), true, "", "Farmácia" },
                    { new Guid("2ded531b-5eeb-4d69-9f7a-90895b315ec1"), true, "", "Açaí" },
                    { new Guid("34c46d06-4521-4959-9ff3-7a2a408ea2d9"), true, "", "Doces e Bolos" },
                    { new Guid("410a6b0f-7355-4e44-8e69-82c74feed28d"), true, "", "Saudável" },
                    { new Guid("72c90639-625d-498c-a510-47b75494cf2b"), true, "", "Brasileira" },
                    { new Guid("9534e443-062c-4d52-a5c3-91ba524e164b"), true, "", "Pizzaria" },
                    { new Guid("a796b547-e119-4b5c-b5d8-72ac98230f52"), true, "", "Lanches" },
                    { new Guid("b0200c03-606b-4180-a682-a407998d572a"), true, "", "Mercado" },
                    { new Guid("f9523ef8-b571-4dac-96a2-715292afb6ea"), true, "", "Bebidas" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CategoryId",
                table: "Stores",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StoreId1",
                table: "Categories",
                column: "StoreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_StoreCategories_CategoryId",
                table: "Stores",
                column: "CategoryId",
                principalTable: "StoreCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_StoreCategories_CategoryId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "StoreCategories");

            migrationBuilder.DropIndex(
                name: "IX_Stores_CategoryId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");
        }
    }
}
