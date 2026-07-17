using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface ICourierLocationReadOnlyRepository
{
    Task<CourierLocation?> GetByCourierUserIdAsync(string courierUserId);
    Task<IList<CourierLocation>> GetAllActiveAsync();
    Task<IReadOnlyList<AvailableCourier>> GetAvailableCouriersAsync(double storeLat, double storeLon, double radiusKm, Guid excludeOrderId);
}

public class AvailableCourier
{
    public string CourierUserId { get; set; } = string.Empty;
    public double? DistanceKm { get; set; }
    public DateTime LastLocationAt { get; set; }
}
