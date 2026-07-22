using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Product;

namespace IOrder.infrastructure.Repositories.Product;

public class CachedProductWriteOnlyRepository : IProductWriteOnlyRepository
{
    private readonly IProductWriteOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedProductWriteOnlyRepository(IProductWriteOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<Domain.Entities.Product> Create(Domain.Entities.Product product)
    {
        var result = await _decorated.Create(product);
        await InvalidateCache(product);
        return result;
    }

    public Domain.Entities.Product Update(Domain.Entities.Product product)
    {
        var result = _decorated.Update(product);
        _ = InvalidateCache(product);
        return result;
    }

    public Domain.Entities.Product Delete(Domain.Entities.Product product)
    {
        var result = _decorated.Delete(product);
        _ = InvalidateCache(product);
        return result;
    }

    public Task<Domain.Entities.Product> GetByIdTracking(Guid id)
    {
        return _decorated.GetByIdTracking(id);
    }

    public Task<IList<Domain.Entities.Product>> GetByIdsTracking(IList<Guid> ids)
    {
        return _decorated.GetByIdsTracking(ids);
    }

    public async Task<PromotionPrice> CreatePromotion(PromotionPrice promotionPrice)
    {
        var result = await _decorated.CreatePromotion(promotionPrice);
        // We could invalidate here if we had the product, but promotionPrice has ProductId.
        _ = _cacheService.RemoveAsync(CacheKeys.ProductById(promotionPrice.ProductId));
        return result;
    }

    public Task<IList<PromotionPrice>> GetPromotionsToStartAsync(DateTime now, CancellationToken cancellationToken)
    {
        return _decorated.GetPromotionsToStartAsync(now, cancellationToken);
    }

    public Task<IList<PromotionPrice>> GetPromotionsToFinishAsync(DateTime now, CancellationToken cancellationToken)
    {
        return _decorated.GetPromotionsToFinishAsync(now, cancellationToken);
    }

    private async Task InvalidateCache(Domain.Entities.Product product)
    {
        await _cacheService.RemoveAsync(CacheKeys.ProductById(product.Id));
        await _cacheService.RemoveAsync(CacheKeys.ProductsByStore(product.StoreId));
    }
}
