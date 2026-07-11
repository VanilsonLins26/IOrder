using IOrder.Domain.Entities.Enums;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Payment : EntityBase, IAggregateRoot
{
    public Guid OrderId { get; init; }
    public Order Order { get; private set; } = null!;
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public string? MercadoPagoPaymentId { get; private set; }
    public string? MercadoPagoPreferenceId { get; private set; }
    public string? PixQrCode { get; private set; }
    public string? PixCopyPaste { get; private set; }
    public string? BoletoUrl { get; private set; }
    public string? BoletoBarcode { get; private set; }
    public string? CardLastFourDigits { get; private set; }
    public int? Installments { get; private set; }
    public string? InstallmentAmount { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; private set; }

    public void SetPixPayment(string mercadoPagoId, string qrCode, string copyPaste)
    {
        Method = PaymentMethod.Pix;
        MercadoPagoPaymentId = mercadoPagoId;
        PixQrCode = qrCode;
        PixCopyPaste = copyPaste;
    }

    public void SetBoletoPayment(string mercadoPagoId, string boletoUrl, string barcode)
    {
        Method = PaymentMethod.Boleto;
        MercadoPagoPaymentId = mercadoPagoId;
        BoletoUrl = boletoUrl;
        BoletoBarcode = barcode;
    }

    public void SetCardPayment(string mercadoPagoId, string lastFourDigits, int installments, string installmentAmount)
    {
        Method = PaymentMethod.CreditCard;
        MercadoPagoPaymentId = mercadoPagoId;
        CardLastFourDigits = lastFourDigits;
        Installments = installments;
        InstallmentAmount = installmentAmount;
    }

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
