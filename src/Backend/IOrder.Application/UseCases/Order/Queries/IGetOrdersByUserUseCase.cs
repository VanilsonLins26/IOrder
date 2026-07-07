using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Order.Queries;

public interface IGetOrdersByUserUseCase
{
    Task<PagedResponse<OrderResponseDto>> Execute(int pageNumber, int pageSize);
}
