using IOrder.Domain.Repositories.Customization;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CustomizationReadOnlyRepositoryBuilder
{
    private readonly Mock<ICustomizationReadOnlyRepository> _repository;

    public CustomizationReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<ICustomizationReadOnlyRepository>();
        _repository.Setup(r => r.GetByProductId(It.IsAny<Guid>()))
            .ReturnsAsync([]);
    }

    public ICustomizationReadOnlyRepository Build() => _repository.Object;
}
