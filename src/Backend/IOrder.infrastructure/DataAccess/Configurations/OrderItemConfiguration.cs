using System.Text.Json;
using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderItems");

        builder.Property(x => x.OrderId)
               .IsRequired();

        builder.Property(x => x.ProductId)
               .IsRequired();

        builder.Property(x => x.ProductName)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.ProductImageUrl)
               .HasMaxLength(500);

        builder.Property(x => x.UnitPrice)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.Quantity)
               .IsRequired();

        builder.Property(x => x.Customize)
               .HasMaxLength(500);

        builder.Property("_imageUrls")
               .HasConversion(new StringListConverter())
               .HasColumnType("json")
               .HasColumnName("ImageUrls");

        builder.Ignore(x => x.TotalPrice);
    }

    private class StringListConverter : ValueConverter<List<string>, string>
    {
        public StringListConverter()
            : base(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>())
        {
        }
    }
}
