using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class CouponConfiguration : BaseEntityConfiguration<Coupon>
{
    public override void Configure(EntityTypeBuilder<Coupon> builder)
    {
        base.Configure(builder);

        builder.ToTable("Coupons");

        builder.Property(x => x.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.DiscountType)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.DiscountValue)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.MaxDiscountAmount)
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.MinPurchaseAmount)
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.ExpiresAt)
               .IsRequired(false);

        builder.Property(x => x.MaxUsageCount)
               .HasDefaultValue(0);

        builder.Property(x => x.CurrentUsageCount)
               .HasDefaultValue(0);
    }
}
