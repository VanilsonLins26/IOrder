using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class CourierLocationConfiguration : IEntityTypeConfiguration<CourierLocation>
{
    public void Configure(EntityTypeBuilder<CourierLocation> builder)
    {
        builder.ToTable("CourierLocations");

        builder.HasKey(x => x.CourierUserId);

        builder.Property(x => x.CourierUserId)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Latitude)
               .HasColumnType("double")
               .IsRequired();

        builder.Property(x => x.Longitude)
               .HasColumnType("double")
               .IsRequired();

        builder.Property(x => x.Location)
               .HasColumnType("point");

        builder.Property(x => x.UpdatedAt)
               .IsRequired();
    }
}
