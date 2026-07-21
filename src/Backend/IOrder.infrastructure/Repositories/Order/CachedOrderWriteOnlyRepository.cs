using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;

namespace IOrder.infrastructure.Repositories.Order;

public class CachedOrderWriteOnlyRepository : IOrderWriteOnlyRepository
{
    private readonly IOrderWriteOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedOrderWriteOnlyRepository(IOrderWriteOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<Domain.Entities.Order> Create(Domain.Entities.Order order)
    {
        var result = await _decorated.Create(order);
        _ = _cacheService.RemoveAsync(CacheKeys.DashboardMetrics(order.StoreId));
        return result;
    }

    public Domain.Entities.Order Update(Domain.Entities.Order order)
    {
        var result = _decorated.Update(order);
        _ = _cacheService.RemoveAsync(CacheKeys.DashboardMetrics(order.StoreId));
        return result;
    }

    public Task<Domain.Entities.Order?> GetByIdTracking(Guid id)
    {
        return _decorated.GetByIdTracking(id);
    }

    public void AddOrderMessage(OrderMessage message)
    {
        _decorated.AddOrderMessage(message);
    }

    public Task MarkMessagesAsReadAsync(Guid orderId, string readByUserId)
    {
        return _decorated.MarkMessagesAsReadAsync(orderId, readByUserId);
    }
}
