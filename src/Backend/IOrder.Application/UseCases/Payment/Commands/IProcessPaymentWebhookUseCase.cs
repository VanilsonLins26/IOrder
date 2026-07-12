using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Payment.Commands;

public interface IProcessPaymentWebhookUseCase
{
    Task<PaymentResponseDto?> Execute(string payload, string? signature);
}
