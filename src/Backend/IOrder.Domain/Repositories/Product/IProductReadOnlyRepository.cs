

using IOrder.Domain.Pagination;
using IOrder.Domain.SeedWork.Pagination;
using System.Runtime.CompilerServices;

namespace IOrder.Domain.Repositories.Product;

public interface IProductReadOnlyRepository
{
    IEnumerable<Entities.Product> GetAll();

    Task<bool> ExistsPromotionInDate(DateTime inicialDate, DateTime finalDate);

    Task<Entities.Product> GetByIdAsync(Guid id);

    Task<PagedList<Entities.Product>> GetAllPagFiltroPrecoAsync(ProductSearchQuery productFilterPrice);

    Task<bool> NameExists(string name);
}
