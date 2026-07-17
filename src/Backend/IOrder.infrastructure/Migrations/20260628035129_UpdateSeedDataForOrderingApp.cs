using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOrder.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataForOrderingApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Bolos de Casamento");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Doces Gourmet");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "Bolo com recheio a escolha e cobertura de pasta americana.", "https://example.com/bolo_casamento.png", "Bolo de Casamento 3 Andares", 350.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "100 unidades de delicioso camafeu fondant com nozes.", "https://example.com/camafeu.png", "Camafeu de Nozes (Cento)", 180.00m });

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Bolos Decorados");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Doces Finos");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Salgados para Festa");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "Cestas de Café da Manhã");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "Name",
                value: "Lembrancinhas Customizadas");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "Name",
                value: "Marmitas Saudáveis (Pré-preparo)");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "Name",
                value: "Tortas Salgadas");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "Name",
                value: "Artesanato");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                column: "Name",
                value: "Kits Festa");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                column: "Name",
                value: "Bebidas Artesanais");

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "Address_Complement", "Address_Street", "Address_ZipCode", "About", "ImageUrl", "Name", "UserId" },
                values: new object[] { "Casa", "Rua das Flores", "12345-001", "Bolos decorados e doces finos sob encomenda para o seu evento.", "https://example.com/doceria.png", "Doceria da Maria", "auth0|maria123" });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address_City", "Address_Complement", "Address_Neighborhood", "Address_Number", "Address_State", "Address_Street", "Address_ZipCode", "About", "Active", "CategoryId", "ImageUrl", "Name", "UserId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000002"), "São Paulo", "Loja 2", "Bela Vista", "200", "SP", "Av. Brasil", "12345-002", "Salgados fritos e assados frescos para sua festa.", true, new Guid("10000000-0000-0000-0000-000000000003"), "https://example.com/salgados.png", "Salgados Express (Sob Encomenda)", "auth0|salgados123" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "São Paulo", "Apto 101", "Jardins", "300", "SP", "Rua do Amor", "12345-003", "Presenteie quem você ama com cestas maravilhosas personalizadas.", true, new Guid("10000000-0000-0000-0000-000000000004"), "https://example.com/cestas.png", "Cestas & Cia", "auth0|cestas123" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Active", "Name", "Position", "StoreId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000003"), true, "Fritos na Hora", 1, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000004"), true, "Tortas e Assados", 2, new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000005"), true, "Cestas Românticas", 1, new Guid("20000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Active", "CategoryId", "CurrentPromotionalPrice", "Customizable", "Description", "ImageUrl", "Name", "Price", "StoreId", "UnitOfMeasure" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000003"), true, new Guid("30000000-0000-0000-0000-000000000003"), null, false, "100 coxinhas de frango para festa, massa de batata.", "https://example.com/coxinha.png", "Cento de Coxinha", 75.00m, new Guid("20000000-0000-0000-0000-000000000002"), "Unidade" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), true, new Guid("30000000-0000-0000-0000-000000000004"), null, true, "Empadão familiar de 2kg com bastante recheio.", "https://example.com/empadao.png", "Empadão de Frango 2kg", 65.00m, new Guid("20000000-0000-0000-0000-000000000002"), "Unidade" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), true, new Guid("30000000-0000-0000-0000-000000000005"), null, true, "Cesta de vime com pães, frutas, sucos, xícara decorada e um ursinho.", "https://example.com/cesta.png", "Cesta de Café da Manhã Amor", 220.00m, new Guid("20000000-0000-0000-0000-000000000003"), "Unidade" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Hambúrgueres");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Bebidas");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "Hambúrguer com carne grelhada.", "https://example.com/whopper.png", "Whopper", 29.90m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                columns: new[] { "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { "Refrigerante lata", "https://example.com/coca.png", "Coca-Cola 350ml", 6.50m });

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Lanches");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Pizzaria");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Açaí");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "Japonês");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"),
                column: "Name",
                value: "Brasileira");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"),
                column: "Name",
                value: "Doces e Bolos");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"),
                column: "Name",
                value: "Farmácia");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"),
                column: "Name",
                value: "Mercado");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000009"),
                column: "Name",
                value: "Bebidas");

            migrationBuilder.UpdateData(
                table: "StoreCategories",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000010"),
                column: "Name",
                value: "Saudável");

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "About", "ImageUrl", "Name", "UserId", "Address_Complement", "Address_Street", "Address_ZipCode" },
                values: new object[] { "O melhor hambúrguer da região.", "https://example.com/bk.png", "Burger King", "auth0|testuser123", "Loja 1", "Av. Principal", "12345-678" });
        }
    }
}
