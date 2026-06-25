using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Repositories.Category;

public interface ICategoryReadOnlyRepository
{
    Task<IList<Domain.Entities.Category>> GetAll(Guid storeId);
    Task<Entities.Category> GetByIdAsync(Guid id);

    Task<bool> NameExists(string name, Guid storeId);
}
