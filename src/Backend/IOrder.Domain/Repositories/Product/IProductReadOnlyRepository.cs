

using IOrder.Domain.Entities.Enums;
using System.Runtime.CompilerServices;

namespace IOrder.Domain.Repositories.Product;

public interface IProductReadOnlyRepository
{
    Task<IList<Entities.Product>> GetAllAsync();

    Task<bool> ExistsPromotionInDate(Guid productId, DateTime inicialDate, DateTime finalDate);

    Task<Entities.Product> GetByIdAsync(Guid id);

    Task<(IList<Entities.Product> Items, int TotalCount)> GetAllPagFiltroPrecoAsync(ProductSearchCriteria criteria);

    Task<bool> NameExists(string name);

    Task<decimal?> GetProductPriceById(Guid productId);

    Task<IDictionary<Guid, decimal>> GetProductPricesByIds(IEnumerable<Guid> productIds);
}
