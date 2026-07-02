using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.Infrastructure.DataAccess.Configurations;

public class PromotionPriceConfiguration : BaseEntityConfiguration<PromotionPrice>
{
    public override void Configure(EntityTypeBuilder<PromotionPrice> builder)
    {
        base.Configure(builder);

        builder.ToTable("PromotionPrices");

        builder.Property(x => x.Price)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.InitialTime).IsRequired();
        builder.Property(x => x.FinalTime).IsRequired();
        builder.HasIndex(x => x.ProductId);
    }
}