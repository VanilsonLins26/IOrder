using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using Moq;

namespace CommomTestUtilities.Repositories.Delivery;

public class DeliveryAssignmentReadOnlyRepositoryBuilder
{
    private readonly Mock<IDeliveryAssignmentReadOnlyRepository> _mock;

    public DeliveryAssignmentReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IDeliveryAssignmentReadOnlyRepository>();
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetByIdAsync(DeliveryAssignment? assignment)
    {
        _mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(assignment);
        return this;
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetByOrderIdAsync(DeliveryAssignment? assignment)
    {
        _mock.Setup(r => r.GetByOrderIdAsync(It.IsAny<Guid>())).ReturnsAsync(assignment);
        return this;
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetByCourierUserIdAsync(List<DeliveryAssignment> assignments)
    {
        _mock.Setup(r => r.GetByCourierUserIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(assignments);
        return this;
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetCountByCourierUserIdAsync(int count)
    {
        _mock.Setup(r => r.GetCountByCourierUserIdAsync(It.IsAny<string>())).ReturnsAsync(count);
        return this;
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetPendingByStoreIdAsync(List<DeliveryAssignment> assignments)
    {
        _mock.Setup(r => r.GetPendingByStoreIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(assignments);
        return this;
    }

    public DeliveryAssignmentReadOnlyRepositoryBuilder GetPendingCountByStoreIdAsync(int count)
    {
        _mock.Setup(r => r.GetPendingCountByStoreIdAsync(It.IsAny<Guid>())).ReturnsAsync(count);
        return this;
    }

    public IDeliveryAssignmentReadOnlyRepository Build() => _mock.Object;
}
