using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Payment;

public interface IPaymentReadOnlyRepository
{
    Task<Entities.Payment?> GetByOrderIdAsync(Guid orderId);
    Task<Entities.Payment?> GetByMercadoPagoIdAsync(string mercadoPagoPaymentId);
}
