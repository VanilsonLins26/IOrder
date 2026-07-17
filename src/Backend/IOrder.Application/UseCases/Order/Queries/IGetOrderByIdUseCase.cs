using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Order.Queries;

public interface IGetOrderByIdUseCase
{
    Task<OrderResponseDto> Execute(Guid id);
}
