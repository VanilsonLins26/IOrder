using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.Infrastructure.DataAccess.Configurations;

public class StoreCategoryConfiguration : BaseEntityConfiguration<StoreCategory>
{
    public override void Configure(EntityTypeBuilder<StoreCategory> builder)
    {
        base.Configure(builder);

        builder.ToTable("StoreCategories");

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.IconUrl).HasMaxLength(255);
    }
}