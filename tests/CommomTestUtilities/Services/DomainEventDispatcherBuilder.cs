using IOrder.Domain.SeedWork;
using IOrder.Domain.Services;
using Moq;

namespace CommomTestUtilities.Services;

public class DomainEventDispatcherBuilder
{
    private readonly Mock<IDomainEventDispatcher> _mock;

    public DomainEventDispatcherBuilder()
    {
        _mock = new Mock<IDomainEventDispatcher>();
    }

    public void BuildDispatchAsync()
    {
        _mock.Setup(x => x.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>())).Returns(Task.CompletedTask);
    }

    public IDomainEventDispatcher Build() => _mock.Object;
}
