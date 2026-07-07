using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Order.Commands;

public interface ICreateOrderUseCase
{
    Task<OrderResponseDto> Execute(Communication.Request.CreateOrderRequestDto request);
}
