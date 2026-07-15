using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Store;
using IOrder.infrastructure.DataAccess;
using IOrder.infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.infrastructure.Repositories.Store;

internal class StoreRepository : IStoreReadOnlyRepository, IStoreWriteOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public StoreRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.Entities.Store> Create(Domain.Entities.Store store)
    {
        await _dbContext.Stores.AddAsync(store);
        return store;
    }

    public Domain.Entities.Store Delete(Domain.Entities.Store store)
    {
        _dbContext.Stores.Remove(store);
        return store;
    }

    public async Task<(IList<Domain.Entities.Store> Items, int TotalCount)> GetAllPaged(StoreSearchCriteria criteria)
    {
        var query = _dbContext.Stores.Include(store => store.OpeningHours).Include(store => store.Category).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.Name))
            query = query.Where(p => p.Name!.Contains(criteria.Name));

        if (criteria.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);

        var isOpenExpr = Domain.Entities.Store.IsOpenExpression();

        var orderedQuery = query.OrderByDescending(isOpenExpr);

        var property = criteria.OrderBy?.ToLower().Trim();

        orderedQuery = property switch
        {
            "name" => criteria.IsDescending
                ? orderedQuery.ThenByDescending(p => p.Name)
                : orderedQuery.ThenBy(p => p.Name),

            _ => criteria.IsDescending
                ? orderedQuery.ThenByDescending(p => p.Id)
                : orderedQuery.ThenBy(p => p.Id)
        };

        var result = await orderedQuery.ToPaginatedTupleAsync(criteria.PageNumber, criteria.PageSize);
        
        if (criteria.UserLatitude.HasValue && criteria.UserLongitude.HasValue)
        {
            var userLocation = new NetTopologySuite.Geometries.Point(criteria.UserLongitude.Value, criteria.UserLatitude.Value) { SRID = 4326 };
            foreach (var store in result.Items)
            {
                if (store.Location != null)
                {
                    // Calculate distance in memory for now, to ensure accurate meters (using Haversine)
                    var distanceMeters = CalculateDistance(criteria.UserLatitude.Value, criteria.UserLongitude.Value, store.Location.Y, store.Location.X);
                    var distanceKm = distanceMeters / 1000.0;
                    
                    store.DistanceKm = distanceKm;
                    store.DeliveryFee = store.BaseDeliveryFee + (store.FeePerKm * (decimal)distanceKm);
                }
            }
        }

        return result;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371e3; // metres
        var phi1 = lat1 * Math.PI / 180;
        var phi2 = lat2 * Math.PI / 180;
        var deltaPhi = (lat2 - lat1) * Math.PI / 180;
        var deltaLambda = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    public async Task<Domain.Entities.Store> GetByIdAsync(Guid id)
    {
        return await _dbContext.Stores.Include(store => store.OpeningHours).Include(store => store.Category).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Domain.Entities.Store?> GetByIdWithDistanceAsync(Guid id, double? userLatitude, double? userLongitude)
    {
        var store = await _dbContext.Stores
            .Include(s => s.OpeningHours)
            .Include(s => s.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (store != null && userLatitude.HasValue && userLongitude.HasValue && store.Location != null)
        {
            var distanceMeters = CalculateDistance(userLatitude.Value, userLongitude.Value, store.Location.Y, store.Location.X);
            var distanceKm = distanceMeters / 1000.0;

            store.DistanceKm = distanceKm;
            store.DeliveryFee = store.BaseDeliveryFee + (store.FeePerKm * (decimal)distanceKm);
        }

        return store;
    }

    public async Task<Domain.Entities.Store> GetByIdTracking(Guid id)
    {
        return await _dbContext.Stores.Include(store => store.OpeningHours).Include(store => store.Category).FirstOrDefaultAsync(s => s.Id == id);
    }



    public async Task<bool> HasStore(string userId)
    {
        return await _dbContext.Stores.AnyAsync(store => store.UserId.Equals(userId));
    }

    public async Task<bool> NameExists(string name)
    {
        return await _dbContext.Stores.AnyAsync(store => store.Name!.Equals(name));
    }

    public async Task<Domain.Entities.Store?> GetByUserIdAsync(string userId)
    {
        return await _dbContext.Stores
            .AsNoTracking()
            .Include(s => s.OpeningHours)
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
