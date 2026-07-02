using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.Infrastructure.DataAccess.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable("Categories");

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.HasOne(c => c.Store)
               .WithMany()
               .HasForeignKey(c => c.StoreId);
        builder.HasIndex(x => x.StoreId);
    }
}