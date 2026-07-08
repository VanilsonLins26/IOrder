using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Services;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}
