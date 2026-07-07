using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class OrderMessageConfiguration : BaseEntityConfiguration<OrderMessage>
{
    public override void Configure(EntityTypeBuilder<OrderMessage> builder)
    {
        base.Configure(builder);

        builder.ToTable("OrderMessages");

        builder.Property(x => x.OrderId)
               .IsRequired();

        builder.Property(x => x.UserId)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.UserRole)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Message)
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(x => x.SentAt)
               .IsRequired();

        builder.Property(x => x.Type)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue(MessageType.Text);

        builder.Property(x => x.ProposedTotalAmount)
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.ProposedDeliveryDate);
    }
}
