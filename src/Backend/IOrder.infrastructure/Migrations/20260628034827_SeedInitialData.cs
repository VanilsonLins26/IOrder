using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("125772b6-55fd-441d-95b2-b418feeb49ef"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("36b1ab6f-dcd2-41f2-85fa-f546ef171304"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("53dd31ea-ce9a-412f-9bcd-9135fe7d6d9b"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("5b669945-c452-402a-8321-3557292b31e5"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("6d335be8-7d81-446e-bae0-8edb561c70a5"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("7ec4e320-13fa-4b2d-9211-e903dae2577f"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("80cd6d01-2113-45ac-97ad-96a2271b5e65"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("99f1b14a-142c-4066-9abc-99fc0325f47a"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("c9e52d68-b51c-48bf-adc3-fe2d19487205"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("d18a4be9-a6d5-4bc1-bfef-0b85c324ade1"));

            migrationBuilder.InsertData(
                table: "StoreCategories",
                columns: new[] { "Id", "Active", "IconUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), true, "", "Lanches" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, "", "Pizzaria" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, "", "Açaí" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, "", "Japonês" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, "", "Brasileira" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), true, "", "Doces e Bolos" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), true, "", "Farmácia" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), true, "", "Mercado" },
                    { new Guid("10000000-0000-0000-0000-000000000009"), true, "", "Bebidas" },
                    { new Guid("10000000-0000-0000-0000-000000000010"), true, "", "Saudável" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address_City", "Address_Complement", "Address_Neighborhood", "Address_Number", "Address_State", "Address_Street", "Address_ZipCode", "About", "Active", "CategoryId", "ImageUrl", "Name", "UserId" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), "São Paulo", "Loja 1", "Centro", "100", "SP", "Av. Principal", "12345-678", "O melhor hambúrguer da região.", true, new Guid("10000000-0000-0000-0000-000000000001"), "https://example.com/bk.png", "Burger King", "auth0|testuser123" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Active", "Name", "Position", "StoreId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), true, "Hambúrgueres", 1, new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), true, "Bebidas", 2, new Guid("20000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Active", "CategoryId", "CurrentPromotionalPrice", "Customizable", "Description", "ImageUrl", "Name", "Price", "StoreId", "UnitOfMeasure" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), true, new Guid("30000000-0000-0000-0000-000000000001"), null, true, "Hambúrguer com carne grelhada.", "https://example.com/whopper.png", "Whopper", 29.90m, new Guid("20000000-0000-0000-0000-000000000001"), "Unidade" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), true, new Guid("30000000-0000-0000-0000-000000000002"), null, false, "Refrigerante lata", "https://example.com/coca.png", "Coca-Cola 350ml", 6.50m, new Guid("20000000-0000-0000-0000-000000000001"), "Unidade" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.InsertData(
                table: "StoreCategories",
                columns: new[] { "Id", "Active", "IconUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("125772b6-55fd-441d-95b2-b418feeb49ef"), true, "", "Lanches" },
                    { new Guid("36b1ab6f-dcd2-41f2-85fa-f546ef171304"), true, "", "Pizzaria" },
                    { new Guid("53dd31ea-ce9a-412f-9bcd-9135fe7d6d9b"), true, "", "Farmácia" },
                    { new Guid("5b669945-c452-402a-8321-3557292b31e5"), true, "", "Mercado" },
                    { new Guid("6d335be8-7d81-446e-bae0-8edb561c70a5"), true, "", "Bebidas" },
                    { new Guid("7ec4e320-13fa-4b2d-9211-e903dae2577f"), true, "", "Saudável" },
                    { new Guid("80cd6d01-2113-45ac-97ad-96a2271b5e65"), true, "", "Doces e Bolos" },
                    { new Guid("99f1b14a-142c-4066-9abc-99fc0325f47a"), true, "", "Brasileira" },
                    { new Guid("c9e52d68-b51c-48bf-adc3-fe2d19487205"), true, "", "Açaí" },
                    { new Guid("d18a4be9-a6d5-4bc1-bfef-0b85c324ade1"), true, "", "Japonês" }
                });
        }
    }
}
