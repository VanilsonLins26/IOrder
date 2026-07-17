using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Payment : EntityBase, IAggregateRoot
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? StripePaymentIntentId { get; set; }
    public string? PixQrCode { get; set; }
    public string? PixCopyPaste { get; set; }
    public string? BoletoUrl { get; set; }
    public string? BoletoBarcode { get; set; }
    public string? CardLastFourDigits { get; set; }
    public int? Installments { get; set; }
    public string? InstallmentAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public void Approve()
    {
        Status = PaymentStatus.Approved;
        PaidAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = PaymentStatus.Rejected;
    }

    public void Refund()
    {
        Status = PaymentStatus.Refunded;
    }

    public void Cancel()
    {
        Status = PaymentStatus.Cancelled;
    }
}
