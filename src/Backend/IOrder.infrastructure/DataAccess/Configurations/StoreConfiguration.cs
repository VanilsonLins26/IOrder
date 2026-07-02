using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class StoreConfiguration : BaseEntityConfiguration<Store>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        base.Configure(builder);
        builder.ToTable("Stores");
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.About).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(255);
        builder.Property(x => x.UserId).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.HasOne(s => s.Category)
               .WithMany()
               .HasForeignKey(s => s.CategoryId);
        builder.OwnsOne(s => s.Address, a =>
        {
            a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(150);
            a.Property(p => p.Number).HasColumnName("Number").HasMaxLength(20);
            a.Property(p => p.Complement).HasColumnName("Complement").HasMaxLength(100);
            a.Property(p => p.Neighborhood).HasColumnName("Neighborhood").HasMaxLength(100);
            a.Property(p => p.City).HasColumnName("City").HasMaxLength(100);
            a.Property(p => p.State).HasColumnName("State").HasMaxLength(50);
            a.Property(p => p.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
        });

        builder.OwnsMany(s => s.OpeningHours, oh =>
        {
            oh.ToTable("OpeningHours"); 
            oh.WithOwner().HasForeignKey("StoreId");
            oh.Property(x => x.DayOfWeek).IsRequired();
        });
    }
}