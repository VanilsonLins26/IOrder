using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class UserCardConfiguration : IEntityTypeConfiguration<UserCard>
{
    public void Configure(EntityTypeBuilder<UserCard> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.GatewayCardId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastFourDigits)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(c => c.Brand)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.ExpirationMonth)
            .IsRequired();
            
        builder.Property(c => c.ExpirationYear)
            .IsRequired();
            
        builder.HasIndex(c => c.UserId);
    }
}
