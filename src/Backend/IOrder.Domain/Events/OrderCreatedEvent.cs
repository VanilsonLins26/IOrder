using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string UserId { get; }
    public Guid StoreId { get; }
    public decimal TotalAmount { get; }
    public DateTime OccurredOn { get; }

    public OrderCreatedEvent(Guid orderId, string userId, Guid storeId, decimal totalAmount)
    {
        OrderId = orderId;
        UserId = userId;
        StoreId = storeId;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}
