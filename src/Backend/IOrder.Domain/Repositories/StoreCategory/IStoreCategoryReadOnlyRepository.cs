using IOrder.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.Domain.Repositories.StoreCategory;

public interface IStoreCategoryReadOnlyRepository
{
    Task<IList<Entities.StoreCategory>> GetAllActive();
}
