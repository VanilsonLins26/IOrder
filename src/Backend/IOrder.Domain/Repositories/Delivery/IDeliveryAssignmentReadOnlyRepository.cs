using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface IDeliveryAssignmentReadOnlyRepository
{
    Task<DeliveryAssignment?> GetByIdAsync(Guid id);
    Task<DeliveryAssignment?> GetByOrderIdAsync(Guid orderId);
    Task<IList<DeliveryAssignment>> GetByCourierUserIdAsync(string courierUserId, int pageNumber, int pageSize);
    Task<int> GetCountByCourierUserIdAsync(string courierUserId);
    Task<IList<DeliveryAssignment>> GetPendingByStoreIdAsync(Guid storeId, int pageNumber, int pageSize);
    Task<int> GetPendingCountByStoreIdAsync(Guid storeId);
}
