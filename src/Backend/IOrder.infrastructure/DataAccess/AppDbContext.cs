using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.DataAccess;

internal class AppDbContext  : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<PromotionPrice> Promotions { get; set; }


}

