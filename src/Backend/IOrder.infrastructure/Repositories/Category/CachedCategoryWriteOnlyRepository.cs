using IOrder.Application.Services.Cache;
using IOrder.Domain.Repositories.Category;

namespace IOrder.infrastructure.Repositories.Category;

public class CachedCategoryWriteOnlyRepository : ICategoryWriteOnlyRepository
{
    private readonly ICategoryWriteOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedCategoryWriteOnlyRepository(ICategoryWriteOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<Domain.Entities.Category> Create(Domain.Entities.Category category)
    {
        var result = await _decorated.Create(category);
        await InvalidateCache(category);
        return result;
    }

    public Domain.Entities.Category Delete(Domain.Entities.Category category)
    {
        var result = _decorated.Delete(category);
        _ = InvalidateCache(category);
        return result;
    }

    public Task<Domain.Entities.Category> GetByIdTracking(Guid id)
    {
        return _decorated.GetByIdTracking(id);
    }

    public void Update(Domain.Entities.Category category)
    {
        _decorated.Update(category);
        _ = InvalidateCache(category);
    }

    private async Task InvalidateCache(Domain.Entities.Category category)
    {
        await _cacheService.RemoveAsync(CacheKeys.CategoriesByStore(category.StoreId));
    }
}
