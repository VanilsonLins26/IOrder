using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Order;
using Mapster;

namespace IOrder.Application.UseCases.Order.Queries;

public class GetOrdersByStoreUseCase : IGetOrdersByStoreUseCase
{
    private readonly IOrderReadOnlyRepository _readOnlyRepository;
    private readonly IStorePermissionService _storePermissionService;

    public GetOrdersByStoreUseCase(
        IOrderReadOnlyRepository readOnlyRepository,
        IStorePermissionService storePermissionService)
    {
        _readOnlyRepository = readOnlyRepository;
        _storePermissionService = storePermissionService;
    }

    public async Task<PagedResponse<OrderResponseDto>> Execute(int pageNumber, int pageSize)
    {
        var storeId = await _storePermissionService.GetLoggedUserStoreIdAsync();

        var orders = await _readOnlyRepository.GetByStoreIdAsync(storeId, pageNumber, pageSize);
        var totalCount = await _readOnlyRepository.GetCountByStoreIdAsync(storeId);

        return new PagedResponse<OrderResponseDto>(orders.Adapt<List<OrderResponseDto>>(), totalCount, pageNumber, pageSize);
    }
}
