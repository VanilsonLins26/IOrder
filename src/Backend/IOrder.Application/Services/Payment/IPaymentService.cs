using IOrder.Communication.Response;

namespace IOrder.Application.Services.Payment;

public interface IPaymentService
{
    Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(Guid orderId, decimal amount, string? customerId);
    Task<PaymentResponseDto?> ProcessWebhookAsync(string payload, string? signature);
    Task<PaymentResponseDto?> GetPaymentByStripeIdAsync(string stripePaymentIntentId);
    Task<string> GetOrCreateCustomerAsync(string email, string name);
    Task DeleteCardAsync(string customerId, string paymentMethodId);
}

public class UserCardDto
{
    public string GatewayCardId { get; set; } = string.Empty;
    public string LastFourDigits { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
}
