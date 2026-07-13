using IOrder.Communication.Response;

namespace IOrder.Application.Services.Payment;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePixPaymentAsync(Guid orderId, decimal amount, string payerEmail, string? payerIdentification);
    Task<PaymentResponseDto> CreateCardPaymentAsync(Guid orderId, decimal amount, string cardToken, int installments, string payerEmail, string? payerIdentification, string? customerId);
    Task<PaymentResponseDto> CreateBoletoPaymentAsync(Guid orderId, decimal amount, string payerEmail, string? payerIdentification);
    Task<PaymentResponseDto?> ProcessWebhookAsync(string payload, string? signature);
    Task<PaymentResponseDto?> GetPaymentByMercadoPagoIdAsync(string mercadoPagoPaymentId);
    Task<string> GetOrCreateCustomerAsync(string email);
    Task<UserCardDto> SaveCardAsync(string customerId, string cardToken);
}

public class UserCardDto
{
    public string GatewayCardId { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
}
