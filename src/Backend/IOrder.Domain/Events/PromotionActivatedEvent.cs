using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class PromotionActivatedEvent : IDomainEvent
{
    public Guid PromotionId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal PromotionalPrice { get; }
    public DateTime OccurredOn { get; }

    public PromotionActivatedEvent(Guid promotionId, Guid productId, string productName, decimal promotionalPrice)
    {
        PromotionId = promotionId;
        ProductId = productId;
        ProductName = productName;
        PromotionalPrice = promotionalPrice;
        OccurredOn = DateTime.UtcNow;
    }
}
