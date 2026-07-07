namespace IOrder.Domain.Repositories.Order;

public interface IOrderWriteOnlyRepository
{
    Task<Entities.Order> Create(Entities.Order order);
    Entities.Order Update(Entities.Order order);
    Task<Entities.Order?> GetByIdTracking(Guid id);
}
