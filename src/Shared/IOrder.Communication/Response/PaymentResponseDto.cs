using IOrder.Communication.Enums;

namespace IOrder.Communication.Response;

public class PaymentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodDto Method { get; set; }
    public PaymentStatusDto Status { get; set; }
    public string? PixQrCode { get; set; }
    public string? PixCopyPaste { get; set; }
    public string? BoletoUrl { get; set; }
    public string? BoletoBarcode { get; set; }
    public string? CardLastFourDigits { get; set; }
    public int? Installments { get; set; }
    public string? InstallmentAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
