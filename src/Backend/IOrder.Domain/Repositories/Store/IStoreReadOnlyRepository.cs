using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Store;

public interface IStoreReadOnlyRepository
{
    Task<(IList<Entities.Store> Items, int TotalCount)> GetAllPaged(StoreSearchCriteria criteria);
    Task<Entities.Store> GetByIdAsync(Guid id);
    Task<bool> NameExists(string name);
    Task<bool> HasStore(string userId);
    Task<Entities.Store?> GetByUserIdAsync(string userId);
}
