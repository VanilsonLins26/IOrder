using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
using Moq;

namespace CommomTestUtilities.Repositories;

public class OrderWriteOnlyRepositoryBuilder
{
    private readonly Mock<IOrderWriteOnlyRepository> _mock;

    public OrderWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IOrderWriteOnlyRepository>();
    }

    public OrderWriteOnlyRepositoryBuilder Create()
    {
        _mock.Setup(r => r.Create(It.IsAny<Order>()))
            .ReturnsAsync((Order order) => order);
        return this;
    }

    public OrderWriteOnlyRepositoryBuilder GetByIdTracking(Order? order)
    {
        _mock.Setup(r => r.GetByIdTracking(It.IsAny<Guid>())).ReturnsAsync(order);
        return this;
    }

    public IOrderWriteOnlyRepository Build() => _mock.Object;
}
