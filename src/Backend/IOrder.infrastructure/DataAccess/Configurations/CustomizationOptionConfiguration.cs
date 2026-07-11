using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class CustomizationOptionConfiguration : BaseEntityConfiguration<CustomizationOption>
{
    public override void Configure(EntityTypeBuilder<CustomizationOption> builder)
    {
        base.Configure(builder);

        builder.ToTable("CustomizationOptions");

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.PriceModifier)
               .HasColumnType("decimal(10,2)")
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(x => x.Position)
               .IsRequired()
               .HasDefaultValue(0);

        builder.HasOne(x => x.Group)
               .WithMany(g => g.Options)
               .HasForeignKey(x => x.GroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
