using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Customization;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CustomizationWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICustomizationWriteOnlyRepository> _mock;

    public CustomizationWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<ICustomizationWriteOnlyRepository>();
    }

    public ICustomizationWriteOnlyRepository Build() => _mock.Object;
}
