using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class CourierRejectedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string CourierUserId { get; }
    public DateTime OccurredOn { get; }

    public CourierRejectedEvent(Guid orderId, string courierUserId)
    {
        OrderId = orderId;
        CourierUserId = courierUserId;
        OccurredOn = DateTime.UtcNow;
    }
}
