using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Events;

public class PaymentRejectedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid PaymentId { get; }
    public decimal Amount { get; }
    public string? RejectionReason { get; }
    public DateTime OccurredOn { get; }

    public PaymentRejectedEvent(Guid orderId, Guid paymentId, decimal amount, string? rejectionReason)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        Amount = amount;
        RejectionReason = rejectionReason;
        OccurredOn = DateTime.UtcNow;
    }
}
