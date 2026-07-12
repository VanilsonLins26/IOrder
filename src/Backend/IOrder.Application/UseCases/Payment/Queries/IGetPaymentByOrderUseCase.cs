using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Payment.Queries;

public interface IGetPaymentByOrderUseCase
{
    Task<PaymentResponseDto?> Execute(Guid orderId);
}
