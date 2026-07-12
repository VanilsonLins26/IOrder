using IOrder.Communication.Response;

namespace IOrder.Application.Services.Payment;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePixPaymentAsync(Guid orderId, decimal amount, string payerEmail, string? payerIdentification);
    Task<PaymentResponseDto> CreateCardPaymentAsync(Guid orderId, decimal amount, string cardToken, int installments, string payerEmail, string? payerIdentification);
    Task<PaymentResponseDto> CreateBoletoPaymentAsync(Guid orderId, decimal amount, string payerEmail, string? payerIdentification);
    Task<PaymentResponseDto?> ProcessWebhookAsync(string payload, string? signature);
    Task<PaymentResponseDto?> GetPaymentByMercadoPagoIdAsync(string mercadoPagoPaymentId);
}
