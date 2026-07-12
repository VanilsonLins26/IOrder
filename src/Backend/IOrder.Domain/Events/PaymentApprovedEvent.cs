using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class PaymentApprovedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid PaymentId { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }

    public PaymentApprovedEvent(Guid orderId, Guid paymentId, decimal amount)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
}
