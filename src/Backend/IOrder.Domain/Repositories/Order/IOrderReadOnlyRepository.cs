namespace IOrder.Domain.Repositories.Order;

public interface IOrderReadOnlyRepository
{
    Task<Entities.Order?> GetByIdAsync(Guid id);
    Task<IList<Entities.Order>> GetByUserIdAsync(string userId, int pageNumber, int pageSize);
    Task<IList<Entities.Order>> GetByStoreIdAsync(Guid storeId, int pageNumber, int pageSize);
    Task<int> GetCountByUserIdAsync(string userId);
    Task<int> GetCountByStoreIdAsync(Guid storeId);
}
