using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Order.Queries;

public interface IGetOrdersByStoreUseCase
{
    Task<PagedResponse<OrderResponseDto>> Execute(int pageNumber, int pageSize);
}
