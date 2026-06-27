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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>()
            .Property(p => p.UnitOfMeasure)
            .HasConversion<string>();

        // Seed Store Categories
        modelBuilder.Entity<StoreCategory>().HasData(
            new StoreCategory { Id = Guid.NewGuid(), Name = "Lanches", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Pizzaria", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Açaí", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Japonês", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Brasileira", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Doces e Bolos", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Farmácia", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Mercado", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Bebidas", IconUrl = "" },
            new StoreCategory { Id = Guid.NewGuid(), Name = "Saudável", IconUrl = "" }
        );
    }


}

