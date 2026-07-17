using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using Mapster;

namespace IOrder.Application.UseCases.Order.Queries;

public class GetOrdersByUserUseCase : IGetOrdersByUserUseCase
{
    private readonly IOrderReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;

    public GetOrdersByUserUseCase(
        IOrderReadOnlyRepository readOnlyRepository,
        ILoggedUserService loggedUserService)
    {
        _readOnlyRepository = readOnlyRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<PagedResponse<OrderResponseDto>> Execute(int pageNumber, int pageSize)
    {
        var userId = _loggedUserService.GetUserId();

        var orders = await _readOnlyRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
        var totalCount = await _readOnlyRepository.GetCountByUserIdAsync(userId);

        return new PagedResponse<OrderResponseDto>
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = orders.Adapt<List<OrderResponseDto>>()
        };
    }
}
