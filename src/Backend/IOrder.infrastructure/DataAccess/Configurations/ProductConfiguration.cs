using IOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.ToTable("Products");
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(255);
        builder.Property(x => x.UnitOfMeasure)
               .HasConversion<string>()
               .HasMaxLength(20);
        builder.Property(x => x.Price)
               .HasColumnType("decimal(10,2)");
        builder.Property(x => x.CurrentPromotionalPrice)
               .HasColumnType("decimal(10,2)");
        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.CategoryId);
    }
}