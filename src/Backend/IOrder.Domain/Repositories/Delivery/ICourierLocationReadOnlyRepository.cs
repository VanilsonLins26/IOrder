using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface ICourierLocationReadOnlyRepository
{
    Task<CourierLocation?> GetByCourierUserIdAsync(string courierUserId);
    Task<IList<CourierLocation>> GetAllActiveAsync();
}
