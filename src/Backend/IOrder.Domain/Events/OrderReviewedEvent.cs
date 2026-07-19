using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class OrderReviewedEvent : IDomainEvent
{
    public Guid ReviewId { get; }
    public Guid OrderId { get; }
    public Guid StoreId { get; }
    public int StoreRating { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public OrderReviewedEvent(Guid reviewId, Guid orderId, Guid storeId, int storeRating)
    {
        ReviewId = reviewId;
        OrderId = orderId;
        StoreId = storeId;
        StoreRating = storeRating;
    }
}
