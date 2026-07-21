using IOrder.Application.Services.Cache;
using IOrder.Domain.Repositories.Store;

namespace IOrder.infrastructure.Repositories.Store;

public class CachedStoreWriteOnlyRepository : IStoreWriteOnlyRepository
{
    private readonly IStoreWriteOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedStoreWriteOnlyRepository(IStoreWriteOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<Domain.Entities.Store> Create(Domain.Entities.Store store)
    {
        var result = await _decorated.Create(store);
        await InvalidateCache(store);
        return result;
    }

    public Domain.Entities.Store Delete(Domain.Entities.Store store)
    {
        var result = _decorated.Delete(store);
        _ = InvalidateCache(store);
        return result;
    }

    public Task<Domain.Entities.Store> GetByIdTracking(Guid id)
    {
        return _decorated.GetByIdTracking(id);
    }

    public void Update(Domain.Entities.Store store)
    {
        _decorated.Update(store);
        _ = InvalidateCache(store);
    }

    private async Task InvalidateCache(Domain.Entities.Store store)
    {
        await _cacheService.RemoveAsync(CacheKeys.StoreById(store.Id));
        await _cacheService.RemoveAsync(CacheKeys.StoreByUserId(store.UserId));
        await _cacheService.RemoveAsync(CacheKeys.DashboardMetrics(store.Id));
    }
}
