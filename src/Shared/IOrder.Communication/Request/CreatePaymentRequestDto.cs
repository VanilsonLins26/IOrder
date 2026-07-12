using IOrder.Communication.Enums;

namespace IOrder.Communication.Request;

public class CreatePaymentRequestDto
{
    public Guid OrderId { get; set; }
    public PaymentMethodDto Method { get; set; }
    public string? CardToken { get; set; }
    public int? Installments { get; set; }
    public string PayerEmail { get; set; } = string.Empty;
    public string? PayerIdentificationType { get; set; }
    public string? PayerIdentificationNumber { get; set; }
}
