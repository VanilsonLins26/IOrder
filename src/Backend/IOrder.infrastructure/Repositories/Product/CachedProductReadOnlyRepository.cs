using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Product;

namespace IOrder.infrastructure.Repositories.Product;

public class CachedProductReadOnlyRepository : IProductReadOnlyRepository
{
    private readonly IProductReadOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedProductReadOnlyRepository(IProductReadOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public Task<IList<Domain.Entities.Product>> GetAllAsync()
    {
        return _decorated.GetAllAsync();
    }

    public Task<bool> ExistsPromotionInDate(Guid productId, DateTime inicialDate, DateTime finalDate)
    {
        return _decorated.ExistsPromotionInDate(productId, inicialDate, finalDate);
    }

    public async Task<Domain.Entities.Product> GetByIdAsync(Guid id)
    {
        var key = CacheKeys.ProductById(id);
        var cached = await _cacheService.GetAsync<Domain.Entities.Product>(key);
        if (cached != null)
        {
            return cached;
        }

        var product = await _decorated.GetByIdAsync(id);
        if (product != null)
        {
            await _cacheService.SetAsync(key, product, TimeSpan.FromMinutes(30));
        }

        return product;
    }

    public Task<(IList<Domain.Entities.Product> Items, int TotalCount)> GetAllPagFiltroPrecoAsync(ProductSearchCriteria criteria)
    {
        return _decorated.GetAllPagFiltroPrecoAsync(criteria);
    }

    public Task<bool> NameExists(string name)
    {
        return _decorated.NameExists(name);
    }

    public async Task<decimal?> GetProductPriceById(Guid productId)
    {
        var key = CacheKeys.ProductById(productId);
        var cached = await _cacheService.GetAsync<Domain.Entities.Product>(key);
        if (cached != null)
        {
            return cached.Price;
        }

        return await _decorated.GetProductPriceById(productId);
    }

    public Task<IDictionary<Guid, decimal>> GetProductPricesByIds(IEnumerable<Guid> productIds)
    {
        return _decorated.GetProductPricesByIds(productIds);
    }
}
