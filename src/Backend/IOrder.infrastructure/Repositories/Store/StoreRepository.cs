using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Pagination;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.SeedWork.Pagination;
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

    public async Task<PagedList<Domain.Entities.Store>> GetAllPaged(StoreSearchQuery storeFilter)
    {
        var query = _dbContext.Stores.Include(store => store.OpeningHours).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(storeFilter.Name))
            query = query.Where(p => p.Name!.Contains(storeFilter.Name));

        if (storeFilter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == storeFilter.CategoryId.Value);

        var currentDay = (int)DateTime.UtcNow.DayOfWeek;
        var previousDay = currentDay == 0 ? 6 : currentDay - 1;
        var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

        var orderedQuery = query.OrderByDescending(s => s.OpeningHours.Any(oh =>
            (oh.DayOfWeek == currentDay && oh.OpenHour <= oh.CloseHour && currentTime >= oh.OpenHour && currentTime <= oh.CloseHour)
            ||
            (oh.DayOfWeek == currentDay && oh.OpenHour > oh.CloseHour && currentTime >= oh.OpenHour)
            ||
            (oh.DayOfWeek == previousDay && oh.OpenHour > oh.CloseHour && currentTime <= oh.CloseHour)
        ));

        var property = storeFilter.OrderBy?.ToLower().Trim();

        orderedQuery = property switch
        {
            "name" => storeFilter.IsDescending
                ? orderedQuery.ThenByDescending(p => p.Name)
                : orderedQuery.ThenBy(p => p.Name),

            _ => storeFilter.IsDescending
                ? orderedQuery.ThenByDescending(p => p.Id)
                : orderedQuery.ThenBy(p => p.Id)
        };

        return await orderedQuery.ToPagedListAsync(storeFilter.PageNumber, storeFilter.PageSize);
    }

    public async Task<Domain.Entities.Store> GetByIdAsync(Guid id)
    {
        return await _dbContext.Stores.Include(store => store.OpeningHours).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Domain.Entities.Store> GetByIdTracking(Guid id)
    {
        return await _dbContext.Stores.Include(store => store.OpeningHours).FirstOrDefaultAsync(s => s.Id == id);
    }

    public void ClearOpeningHours(Domain.Entities.Store store)
    {
        _dbContext.Set<Domain.Entities.OpeningHour>().RemoveRange(store.OpeningHours);
        store.OpeningHours.Clear();
    }

    public void DeleteOpeningHour(Domain.Entities.OpeningHour openingHour)
    {
        _dbContext.Set<Domain.Entities.OpeningHour>().Remove(openingHour);
    }

    public void AddOpeningHour(Domain.Entities.OpeningHour openingHour)
    {
        _dbContext.Set<Domain.Entities.OpeningHour>().Add(openingHour);
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
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
}
