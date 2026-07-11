using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class OrderStatusChangedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string UserId { get; }
    public Guid StoreId { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public DateTime OccurredOn { get; }

    public OrderStatusChangedEvent(Guid orderId, string userId, Guid storeId, OrderStatus oldStatus, OrderStatus newStatus)
    {
        OrderId = orderId;
        UserId = userId;
        StoreId = storeId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }
}
