using IOrder.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Store;

public interface IStoreWriteOnlyRepository
{
    Task<Entities.Store> Create(Entities.Store store);
    Entities.Store Delete(Entities.Store store);
    Task<Entities.Store> GetByIdTracking(Guid id);
    void ClearOpeningHours(Entities.Store store);
    void DeleteOpeningHour(Entities.OpeningHour openingHour);
    void AddOpeningHour(Entities.OpeningHour openingHour);
}
