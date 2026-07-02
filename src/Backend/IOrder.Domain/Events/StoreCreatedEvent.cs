using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class StoreCreatedEvent : IDomainEvent
{
    public Guid StoreId { get; }
    public string StoreName { get; }
    public DateTime OccurredOn { get; }

    public StoreCreatedEvent(Guid storeId, string storeName)
    {
        StoreId = storeId;
        StoreName = storeName;
        OccurredOn = DateTime.UtcNow;
    }
}
