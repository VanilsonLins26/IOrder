using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<OrderReview>
{
    public void Configure(EntityTypeBuilder<OrderReview> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.StoreRating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(500)
            .IsRequired(false);

        // Um pedido pode ter apenas uma avaliação
        builder.HasIndex(r => r.OrderId)
            .IsUnique();

        builder.HasOne(r => r.Order)
            .WithOne()
            .HasForeignKey<OrderReview>(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Store)
            .WithMany()
            .HasForeignKey(r => r.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
