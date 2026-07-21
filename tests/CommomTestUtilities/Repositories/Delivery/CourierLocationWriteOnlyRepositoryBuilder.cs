using IOrder.Domain.Repositories.Delivery;
using Moq;

namespace CommomTestUtilities.Repositories.Delivery;

public class CourierLocationWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICourierLocationWriteOnlyRepository> _mock;

    public CourierLocationWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<ICourierLocationWriteOnlyRepository>();
    }

    public ICourierLocationWriteOnlyRepository Build() => _mock.Object;
}
