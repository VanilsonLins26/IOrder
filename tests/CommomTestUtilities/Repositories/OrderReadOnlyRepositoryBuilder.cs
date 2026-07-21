using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
using Moq;

namespace CommomTestUtilities.Repositories;

public class OrderReadOnlyRepositoryBuilder
{
    private readonly Mock<IOrderReadOnlyRepository> _mock;

    public OrderReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IOrderReadOnlyRepository>();
    }

    public OrderReadOnlyRepositoryBuilder GetByIdAsync(Order? order)
    {
        _mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(order);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetByUserIdAsync(List<Order> orders)
    {
        _mock.Setup(r => r.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(orders);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetByStoreIdAsync(List<Order> orders)
    {
        _mock.Setup(r => r.GetByStoreIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(orders);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetCountByUserIdAsync(int count)
    {
        _mock.Setup(r => r.GetCountByUserIdAsync(It.IsAny<string>())).ReturnsAsync(count);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetCountByStoreIdAsync(int count)
    {
        _mock.Setup(r => r.GetCountByStoreIdAsync(It.IsAny<Guid>())).ReturnsAsync(count);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetEligibleForAutoSearchAsync(List<Order> orders)
    {
        _mock.Setup(r => r.GetEligibleForAutoSearchAsync()).ReturnsAsync(orders);
        return this;
    }

    public OrderReadOnlyRepositoryBuilder GetAvailableForDeliveryAsync(List<Order> orders)
    {
        _mock.Setup(r => r.GetAvailableForDeliveryAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(orders);
        return this;
    }

    public IOrderReadOnlyRepository Build() => _mock.Object;
}
