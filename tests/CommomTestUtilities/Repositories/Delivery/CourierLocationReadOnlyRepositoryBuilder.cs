using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using Moq;

namespace CommomTestUtilities.Repositories.Delivery;

public class CourierLocationReadOnlyRepositoryBuilder
{
    private readonly Mock<ICourierLocationReadOnlyRepository> _mock;

    public CourierLocationReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<ICourierLocationReadOnlyRepository>();
    }

    public CourierLocationReadOnlyRepositoryBuilder GetByCourierUserIdAsync(CourierLocation? location)
    {
        _mock.Setup(r => r.GetByCourierUserIdAsync(It.IsAny<string>())).ReturnsAsync(location);
        return this;
    }

    public CourierLocationReadOnlyRepositoryBuilder GetAllActiveAsync(List<CourierLocation> locations)
    {
        _mock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(locations);
        return this;
    }

    public CourierLocationReadOnlyRepositoryBuilder GetAvailableCouriersAsync(List<AvailableCourier> couriers)
    {
        _mock.Setup(r => r.GetAvailableCouriersAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<Guid>()))
            .ReturnsAsync(couriers);
        return this;
    }

    public ICourierLocationReadOnlyRepository Build() => _mock.Object;
}
