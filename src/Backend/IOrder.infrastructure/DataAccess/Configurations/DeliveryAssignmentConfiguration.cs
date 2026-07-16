using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOrder.infrastructure.DataAccess.Configurations;

public class DeliveryAssignmentConfiguration : BaseEntityConfiguration<DeliveryAssignment>
{
    public override void Configure(EntityTypeBuilder<DeliveryAssignment> builder)
    {
        base.Configure(builder);

        builder.ToTable("DeliveryAssignments");

        builder.Property(x => x.OrderId)
               .IsRequired();

        builder.Property(x => x.CourierUserId)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired()
               .HasDefaultValue(AssignmentStatus.Pending);

        builder.Property(x => x.AssignedAt)
               .IsRequired();

        builder.Property(x => x.AcceptedAt);

        builder.Property(x => x.PickedUpAt);

        builder.Property(x => x.DeliveredAt);

        builder.Property(x => x.CourierNotes)
               .HasMaxLength(1000);

        builder.HasOne(x => x.Order)
               .WithMany(o => o.Assignments)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.CourierUserId);
        builder.HasIndex(x => x.Status);
    }
}
