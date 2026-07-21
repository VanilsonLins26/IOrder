using IOrder.Application.Services.Cache;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Dashboard;

namespace IOrder.infrastructure.Repositories.Dashboard;

public class CachedDashboardReadOnlyRepository : IDashboardReadOnlyRepository
{
    private readonly IDashboardReadOnlyRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedDashboardReadOnlyRepository(IDashboardReadOnlyRepository decorated, ICacheService cacheService)
    {
        _decorated = decorated;
        _cacheService = cacheService;
    }

    public async Task<StoreDashboardSummary> GetStoreDashboardMetricsAsync(Guid storeId)
    {
        var key = CacheKeys.DashboardMetrics(storeId);
        var cached = await _cacheService.GetAsync<StoreDashboardSummary>(key);
        if (cached != null)
        {
            return cached;
        }

        var metrics = await _decorated.GetStoreDashboardMetricsAsync(storeId);
        if (metrics != null)
        {
            await _cacheService.SetAsync(key, metrics, TimeSpan.FromMinutes(5));
        }

        return metrics ?? new StoreDashboardSummary();
    }
}
