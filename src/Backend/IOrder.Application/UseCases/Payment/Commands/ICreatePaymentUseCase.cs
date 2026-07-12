using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Payment.Commands;

public interface ICreatePaymentUseCase
{
    Task<PaymentResponseDto> Execute(CreatePaymentRequestDto request);
}
