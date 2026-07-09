using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class PromotionDeactivatedEvent : IDomainEvent
{
    public Guid PromotionId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public DateTime OccurredOn { get; }

    public PromotionDeactivatedEvent(Guid promotionId, Guid productId, string productName)
    {
        PromotionId = promotionId;
        ProductId = productId;
        ProductName = productName;
        OccurredOn = DateTime.UtcNow;
    }
}
