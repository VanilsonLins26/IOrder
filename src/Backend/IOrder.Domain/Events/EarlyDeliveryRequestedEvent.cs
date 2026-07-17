using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class EarlyDeliveryRequestedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string UserId { get; }
    public DateTime OccurredOn { get; }

    public EarlyDeliveryRequestedEvent(Guid orderId, string userId)
    {
        OrderId = orderId;
        UserId = userId;
        OccurredOn = DateTime.UtcNow;
    }
}
