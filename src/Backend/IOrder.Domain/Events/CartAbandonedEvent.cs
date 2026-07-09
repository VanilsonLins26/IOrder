using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class CartAbandonedEvent : IDomainEvent
{
    public string UserId { get; }
    public string UserEmail { get; }
    public DateTime OccurredOn { get; }

    public CartAbandonedEvent(string userId, string userEmail)
    {
        UserId = userId;
        UserEmail = userEmail;
        OccurredOn = DateTime.UtcNow;
    }
}
