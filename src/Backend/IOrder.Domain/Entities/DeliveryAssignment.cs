using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Events;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class DeliveryAssignment : EntityBase, IAggregateRoot
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string CourierUserId { get; set; } = string.Empty;
    public AssignmentStatus Status { get; private set; } = AssignmentStatus.Pending;
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? CourierNotes { get; set; }

    public void Accept()
    {
        Status = AssignmentStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        AddDomainEvent(new CourierAcceptedEvent(OrderId, CourierUserId));
    }

    public void Reject()
    {
        Status = AssignmentStatus.Rejected;
        AddDomainEvent(new CourierRejectedEvent(OrderId, CourierUserId));
    }

    public void Pickup()
    {
        Status = AssignmentStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
        AddDomainEvent(new OrderPickedUpEvent(OrderId, CourierUserId));
    }

    public void StartTransit()
    {
        Status = AssignmentStatus.InTransit;
    }

    public void Deliver()
    {
        Status = AssignmentStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        AddDomainEvent(new OrderDeliveredByCourierEvent(OrderId, CourierUserId));
    }

    public void Fail()
    {
        Status = AssignmentStatus.Failed;
    }
}
