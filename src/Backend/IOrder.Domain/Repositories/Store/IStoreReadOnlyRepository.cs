using IOrder.Domain.Entities;
using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Store;

public interface IStoreReadOnlyRepository
{
    Task<PagedList<Entities.Store>> GetAllPaged(StoreSearchQuery storeFilter);
    Task<Entities.Store> GetByIdAsync(Guid id);
    Task<bool> NameExists(string name);
    Task<bool> HasStore(string userId);
}
