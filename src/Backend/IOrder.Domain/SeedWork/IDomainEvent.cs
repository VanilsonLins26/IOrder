namespace IOrder.Domain.SeedWork;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
