using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class CustomizationGroupConfiguration : BaseEntityConfiguration<CustomizationGroup>
{
    public override void Configure(EntityTypeBuilder<CustomizationGroup> builder)
    {
        base.Configure(builder);

        builder.ToTable("CustomizationGroups");

        builder.Property(x => x.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Type)
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(x => x.MinSelections)
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(x => x.MaxSelections)
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(x => x.Required)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(x => x.Position)
               .IsRequired()
               .HasDefaultValue(0);

        builder.HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Options)
               .WithOne(o => o.Group)
               .HasForeignKey(o => o.GroupId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
