using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using Moq;

namespace CommomTestUtilities.Repositories.Delivery;

public class DeliveryAssignmentWriteOnlyRepositoryBuilder
{
    private readonly Mock<IDeliveryAssignmentWriteOnlyRepository> _mock;

    public DeliveryAssignmentWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IDeliveryAssignmentWriteOnlyRepository>();
    }

    public DeliveryAssignmentWriteOnlyRepositoryBuilder GetByIdTrackingAsync(DeliveryAssignment? assignment)
    {
        _mock.Setup(r => r.GetByIdTrackingAsync(It.IsAny<Guid>())).ReturnsAsync(assignment);
        return this;
    }

    public IDeliveryAssignmentWriteOnlyRepository Build() => _mock.Object;
}
