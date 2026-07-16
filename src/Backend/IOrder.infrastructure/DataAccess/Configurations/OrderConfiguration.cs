using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class OrderConfiguration : BaseEntityConfiguration<Domain.Entities.Order>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("Orders");

        builder.Property(x => x.UserId)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.CustomerEmail)
               .HasMaxLength(200);

        builder.Property(x => x.CustomerPhone)
               .HasMaxLength(20);

        builder.Property(x => x.StoreId)
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue(OrderStatus.Pending);

        builder.Property(x => x.TotalAmount)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.OriginalAmount)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.CouponCode)
               .HasMaxLength(50);

        builder.Property(x => x.DiscountValue)
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.DiscountedTotal)
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.DeliveryDate);

        builder.Property(x => x.DeliveryType)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue(DeliveryType.Delivery);

        builder.Property(x => x.DeliveryFee)
               .HasColumnType("decimal(10,2)")
               .IsRequired()
               .HasDefaultValue(0m);

        builder.Property(x => x.CustomerNotes)
               .HasMaxLength(1000);

        builder.Property(x => x.ShopkeeperNotes)
               .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        builder.HasMany(x => x.Items)
               .WithOne(x => x.Order)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Messages)
               .WithOne(x => x.Order)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.LastMessageAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.LastMessageAt);
    }
}
