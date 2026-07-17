using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Order;

public interface IOrderWriteOnlyRepository
{
    Task<Entities.Order> Create(Entities.Order order);
    Entities.Order Update(Entities.Order order);
    Task<Entities.Order?> GetByIdTracking(Guid id);
    void AddOrderMessage(OrderMessage message);
    Task MarkMessagesAsReadAsync(Guid orderId, string readByUserId);
}
