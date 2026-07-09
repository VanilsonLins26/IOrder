using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class CartAbandonedEvent : IDomainEvent
{
    public string UserId { get; }
    public string? UserPhone { get; }
    public DateTime OccurredOn { get; }

    public CartAbandonedEvent(string userId, string? userPhone)
    {
        UserId = userId;
        UserPhone = userPhone;
        OccurredOn = DateTime.UtcNow;
    }
}
