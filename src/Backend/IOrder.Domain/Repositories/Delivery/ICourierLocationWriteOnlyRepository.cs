using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Delivery;

public interface ICourierLocationWriteOnlyRepository
{
    Task UpsertAsync(CourierLocation location);
}
