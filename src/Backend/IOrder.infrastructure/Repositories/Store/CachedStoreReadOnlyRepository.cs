using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Store;

namespace IOrder.infrastructure.Repositories.Store;

public class CachedStoreReadOnlyRepository : IStoreReadOnlyRepository
{
    private readonly IStoreReadOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedStoreReadOnlyRepository(IStoreReadOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<(IList<Domain.Entities.Store> Items, int TotalCount)> GetAllPaged(StoreSearchCriteria criteria)
    {
        return await _decorated.GetAllPaged(criteria);
    }

    public async Task<Domain.Entities.Store> GetByIdAsync(Guid id)
    {
        var key = CacheKeys.StoreById(id);
        var cached = await _cacheService.GetAsync<Domain.Entities.Store>(key);
        if (cached != null)
        {
            return cached;
        }

        var store = await _decorated.GetByIdAsync(id);
        if (store != null)
        {
            await _cacheService.SetAsync(key, store, TimeSpan.FromHours(1));
        }

        return store;
    }

    public Task<Domain.Entities.Store?> GetByIdWithDistanceAsync(Guid id, double? userLatitude, double? userLongitude)
    {
        return _decorated.GetByIdWithDistanceAsync(id, userLatitude, userLongitude);
    }

    public Task<bool> NameExists(string name)
    {
        return _decorated.NameExists(name);
    }

    public Task<bool> HasStore(string userId)
    {
        return _decorated.HasStore(userId);
    }

    public async Task<Domain.Entities.Store?> GetByUserIdAsync(string userId)
    {
        var key = CacheKeys.StoreByUserId(userId);
        var cached = await _cacheService.GetAsync<Domain.Entities.Store>(key);
        if (cached != null)
        {
            return cached;
        }

        var store = await _decorated.GetByUserIdAsync(userId);
        if (store != null)
        {
            await _cacheService.SetAsync(key, store, TimeSpan.FromHours(1));
        }

        return store;
    }
}
