using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Category;

namespace IOrder.infrastructure.Repositories.Category;

public class CachedCategoryReadOnlyRepository : ICategoryReadOnlyRepository
{
    private readonly ICategoryReadOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedCategoryReadOnlyRepository(ICategoryReadOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<IList<Domain.Entities.Category>> GetAll(Guid storeId)
    {
        var key = CacheKeys.CategoriesByStore(storeId);
        var cached = await _cacheService.GetAsync<IList<Domain.Entities.Category>>(key);
        if (cached != null)
        {
            return cached;
        }

        var categories = await _decorated.GetAll(storeId);
        if (categories != null)
        {
            await _cacheService.SetAsync(key, categories, TimeSpan.FromHours(1));
        }

        return categories ?? new List<Domain.Entities.Category>();
    }

    public Task<Domain.Entities.Category> GetByIdAsync(Guid id)
    {
        return _decorated.GetByIdAsync(id);
    }

    public Task<bool> NameExists(string name, Guid storeId)
    {
        return _decorated.NameExists(name, storeId);
    }
}
