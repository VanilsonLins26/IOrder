using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class OrderPickedUpEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string CourierUserId { get; }
    public DateTime OccurredOn { get; }

    public OrderPickedUpEvent(Guid orderId, string courierUserId)
    {
        OrderId = orderId;
        CourierUserId = courierUserId;
        OccurredOn = DateTime.UtcNow;
    }
}
