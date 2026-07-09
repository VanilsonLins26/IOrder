using IOrder.Domain.SeedWork;
using System;

namespace IOrder.Domain.Events;

public class NewOrderMessageEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string SenderUserId { get; }
    public string MessageText { get; }
    public DateTime OccurredOn { get; }

    public NewOrderMessageEvent(Guid orderId, string senderUserId, string messageText)
    {
        OrderId = orderId;
        SenderUserId = senderUserId;
        MessageText = messageText;
        OccurredOn = DateTime.UtcNow;
    }
}
