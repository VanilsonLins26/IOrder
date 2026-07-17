using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Payment;

public interface IPaymentWriteOnlyRepository
{
    Task CreateAsync(Entities.Payment payment);
    Entities.Payment Update(Entities.Payment payment);
    Task<Entities.Payment?> GetByIdTracking(Guid id);
}
