using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class PriceChangedEvent : IDomainEvent
{
    public Guid ProductId { get; }
    public decimal NewPrice { get; }
    public DateTime OccurredOn { get; }

    public PriceChangedEvent(Guid productId, decimal newPrice)
    {
        ProductId = productId;
        NewPrice = newPrice;
        OccurredOn = DateTime.UtcNow;
    }
}
