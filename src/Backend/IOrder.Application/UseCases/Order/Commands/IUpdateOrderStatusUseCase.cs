using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Order.Commands;

public interface IUpdateOrderStatusUseCase
{
    Task<OrderResponseDto> Execute(Guid id, Communication.Request.UpdateOrderStatusRequestDto request);
}
