using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Stores_StoreId1",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_StoreId1",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("1dee1a76-b7d5-429c-9250-70a735dcc297"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("1e88ad1a-c9e8-4d86-8e0b-3da38d55650e"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("2ded531b-5eeb-4d69-9f7a-90895b315ec1"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("34c46d06-4521-4959-9ff3-7a2a408ea2d9"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("410a6b0f-7355-4e44-8e69-82c74feed28d"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("72c90639-625d-498c-a510-47b75494cf2b"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("9534e443-062c-4d52-a5c3-91ba524e164b"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("a796b547-e119-4b5c-b5d8-72ac98230f52"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("b0200c03-606b-4180-a682-a407998d572a"));

            migrationBuilder.DeleteData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("f9523ef8-b571-4dac-96a2-715292afb6ea"));

            migrationBuilder.DropColumn(
                name: "StoreId1",
                table: "Categories");

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

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StoreId",
                table: "Categories",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_StoreId",
                table: "Categories");

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

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId1",
                table: "Categories",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

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
                name: "IX_Categories_StoreId1",
                table: "Categories",
                column: "StoreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Stores_StoreId1",
                table: "Categories",
                column: "StoreId1",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
