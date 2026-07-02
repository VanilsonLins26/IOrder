using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebApi.Test")]
namespace IOrder.infrastructure.DataAccess;

internal class AppDbContext  : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<PromotionPrice> Promotions { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<StoreCategory> StoreCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);


        var catBolosId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var catDocesId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var catSalgadosId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var catCestasId = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var catLembrancinhasId = Guid.Parse("10000000-0000-0000-0000-000000000005");
        var catMarmitasId = Guid.Parse("10000000-0000-0000-0000-000000000006");
        var catTortasId = Guid.Parse("10000000-0000-0000-0000-000000000007");
        var catArtesanatoId = Guid.Parse("10000000-0000-0000-0000-000000000008");
        var catKitsFestaId = Guid.Parse("10000000-0000-0000-0000-000000000009");
        var catBebidasArtesanaisId = Guid.Parse("10000000-0000-0000-0000-000000000010");

        modelBuilder.Entity<StoreCategory>().HasData(
            new StoreCategory { Id = catBolosId, Name = "Bolos Decorados", IconUrl = "" },
            new StoreCategory { Id = catDocesId, Name = "Doces Finos", IconUrl = "" },
            new StoreCategory { Id = catSalgadosId, Name = "Salgados para Festa", IconUrl = "" },
            new StoreCategory { Id = catCestasId, Name = "Cestas de Café da Manhã", IconUrl = "" },
            new StoreCategory { Id = catLembrancinhasId, Name = "Lembrancinhas Customizadas", IconUrl = "" },
            new StoreCategory { Id = catMarmitasId, Name = "Marmitas Saudáveis (Pré-preparo)", IconUrl = "" },
            new StoreCategory { Id = catTortasId, Name = "Tortas Salgadas", IconUrl = "" },
            new StoreCategory { Id = catArtesanatoId, Name = "Artesanato", IconUrl = "" },
            new StoreCategory { Id = catKitsFestaId, Name = "Kits Festa", IconUrl = "" },
            new StoreCategory { Id = catBebidasArtesanaisId, Name = "Bebidas Artesanais", IconUrl = "" }
        );

        var storeMariaId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var storeSalgadosId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var storeCestasId = Guid.Parse("20000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Store>().HasData(
            new Store 
            { 
                Id = storeMariaId, 
                Name = "Doceria da Maria", 
                About = "Bolos decorados e doces finos sob encomenda para o seu evento.",
                ImageUrl = "https://images.unsplash.com/photo-1559598467-f8b76c8155d0?w=500", 
                CategoryId = catBolosId,
                UserId = "auth0|maria123"
            },
            new Store 
            { 
                Id = storeSalgadosId, 
                Name = "Salgados Express (Sob Encomenda)", 
                About = "Salgados fritos e assados frescos para sua festa.",
                ImageUrl = "https://images.unsplash.com/photo-1626082895617-2c6ab3abfa01?w=500", 
                CategoryId = catSalgadosId,
                UserId = "auth0|salgados123"
            },
            new Store 
            { 
                Id = storeCestasId, 
                Name = "Cestas & Cia", 
                About = "Presenteie quem você ama com cestas maravilhosas personalizadas.",
                ImageUrl = "https://images.unsplash.com/photo-1549465220-1a8b9238cd48?w=500", 
                CategoryId = catCestasId,
                UserId = "auth0|cestas123"
            }
        );
        

        modelBuilder.Entity<Store>().OwnsOne(s => s.Address).HasData(
            new { StoreId = storeMariaId, ZipCode = "12345-001", Street = "Rua das Flores", Number = "100", Complement = "Casa", Neighborhood = "Centro", City = "São Paulo", State = "SP" },
            new { StoreId = storeSalgadosId, ZipCode = "12345-002", Street = "Av. Brasil", Number = "200", Complement = "Loja 2", Neighborhood = "Bela Vista", City = "São Paulo", State = "SP" },
            new { StoreId = storeCestasId, ZipCode = "12345-003", Street = "Rua do Amor", Number = "300", Complement = "Apto 101", Neighborhood = "Jardins", City = "São Paulo", State = "SP" }
        );

        var menuBolosId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var menuDocesId = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var menuFritosId = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var menuAssadosId = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var menuRomanticasId = Guid.Parse("30000000-0000-0000-0000-000000000005");

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = menuBolosId, Name = "Bolos de Casamento", Position = 1, StoreId = storeMariaId },
            new Category { Id = menuDocesId, Name = "Doces Gourmet", Position = 2, StoreId = storeMariaId },
            new Category { Id = menuFritosId, Name = "Fritos na Hora", Position = 1, StoreId = storeSalgadosId },
            new Category { Id = menuAssadosId, Name = "Tortas e Assados", Position = 2, StoreId = storeSalgadosId },
            new Category { Id = menuRomanticasId, Name = "Cestas Românticas", Position = 1, StoreId = storeCestasId }
        );


        modelBuilder.Entity<Product>().HasData(
            new { 
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), 
                Name = "Bolo de Casamento 3 Andares", 
                Description = "Bolo com recheio a escolha e cobertura de pasta americana.",
                Price = 350.00m, UnitOfMeasure = Domain.Entities.Enums.UnitOfMeasure.Unidade, ImageUrl = "https://images.unsplash.com/photo-1535254973040-607b474cb50d?w=500",
                StoreId = storeMariaId, CategoryId = menuBolosId, Customizable = true
            },
            new { 
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), 
                Name = "Camafeu de Nozes (Cento)", 
                Description = "100 unidades de delicioso camafeu fondant com nozes.",
                Price = 180.00m, UnitOfMeasure = Domain.Entities.Enums.UnitOfMeasure.Unidade, ImageUrl = "https://images.unsplash.com/photo-1587314168485-3236d6710814?w=500",
                StoreId = storeMariaId, CategoryId = menuDocesId, Customizable = false
            },
            new { 
                Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), 
                Name = "Cento de Coxinha", 
                Description = "100 coxinhas de frango para festa, massa de batata.",
                Price = 75.00m, UnitOfMeasure = Domain.Entities.Enums.UnitOfMeasure.Unidade, ImageUrl = "https://images.unsplash.com/photo-1628198755050-482eebe9b165?w=500",
                StoreId = storeSalgadosId, CategoryId = menuFritosId, Customizable = false
            },
            new { 
                Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), 
                Name = "Empadão de Frango 2kg", 
                Description = "Empadão familiar de 2kg com bastante recheio.",
                Price = 65.00m, UnitOfMeasure = Domain.Entities.Enums.UnitOfMeasure.Unidade, ImageUrl = "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=500",
                StoreId = storeSalgadosId, CategoryId = menuAssadosId, Customizable = true
            },
            new { 
                Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), 
                Name = "Cesta de Café da Manhã Amor", 
                Description = "Cesta de vime com pães, frutas, sucos, xícara decorada e um ursinho.",
                Price = 220.00m, UnitOfMeasure = Domain.Entities.Enums.UnitOfMeasure.Unidade, ImageUrl = "https://images.unsplash.com/photo-1528659101188-11116c4832ce?w=500",
                StoreId = storeCestasId, CategoryId = menuRomanticasId, Customizable = true
            }
        );
    }


}

