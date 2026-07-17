using IOrder.Communication.Response;
using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface ICourierLocationReadOnlyRepository
{
    Task<CourierLocation?> GetByCourierUserIdAsync(string courierUserId);
    Task<IList<CourierLocation>> GetAllActiveAsync();
    Task<IReadOnlyList<AvailableCourierResponseDto>> GetAvailableCouriersAsync(double storeLat, double storeLon, double radiusKm, Guid excludeOrderId);
}
