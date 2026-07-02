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

        return await orderedQuery.ToPaginatedTupleAsync(criteria.PageNumber, criteria.PageSize);
    }

    public async Task<Domain.Entities.Store> GetByIdAsync(Guid id)
    {
        return await _dbContext.Stores.Include(store => store.OpeningHours).Include(store => store.Category).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
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
