using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class PaymentConfiguration : BaseEntityConfiguration<Domain.Entities.Payment>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.Payment> builder)
    {
        base.Configure(builder);

        builder.ToTable("Payments");

        builder.Property(x => x.OrderId)
               .IsRequired();

        builder.Property(x => x.Amount)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.Property(x => x.Method)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(x => x.MercadoPagoPaymentId)
               .HasMaxLength(100);

        builder.Property(x => x.MercadoPagoPreferenceId)
               .HasMaxLength(100);

        builder.Property(x => x.PixQrCode)
               .HasColumnType("text");

        builder.Property(x => x.PixCopyPaste)
               .HasColumnType("text");

        builder.Property(x => x.BoletoUrl)
               .HasMaxLength(500);

        builder.Property(x => x.BoletoBarcode)
               .HasMaxLength(100);

        builder.Property(x => x.CardLastFourDigits)
               .HasMaxLength(4);

        builder.Property(x => x.Installments);

        builder.Property(x => x.InstallmentAmount)
               .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.PaidAt);

        builder.HasOne(x => x.Order)
               .WithMany()
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrderId)
               .IsUnique();

        builder.HasIndex(x => x.MercadoPagoPaymentId);
    }
}
