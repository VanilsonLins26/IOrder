using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.Infrastructure.DataAccess.Configurations;

public class UserProfileConfiguration : BaseEntityConfiguration<UserProfile>
{
    public override void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserProfiles");

        builder.Property(x => x.UserId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20);

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}
