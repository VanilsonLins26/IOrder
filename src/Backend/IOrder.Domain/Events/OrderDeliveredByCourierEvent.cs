using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class OrderDeliveredByCourierEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string CourierUserId { get; }
    public DateTime OccurredOn { get; }

    public OrderDeliveredByCourierEvent(Guid orderId, string courierUserId)
    {
        OrderId = orderId;
        CourierUserId = courierUserId;
        OccurredOn = DateTime.UtcNow;
    }
}
