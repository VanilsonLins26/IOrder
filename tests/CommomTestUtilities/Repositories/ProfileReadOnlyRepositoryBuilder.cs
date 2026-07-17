using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ProfileReadOnlyRepositoryBuilder
{
    private readonly Mock<IProfileReadOnlyRepository> _mock;

    public ProfileReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IProfileReadOnlyRepository>();
    }

    public ProfileReadOnlyRepositoryBuilder GetByUserId(UserProfile? profile)
    {
        _mock.Setup(r => r.GetByUserId(It.IsAny<string>())).ReturnsAsync(profile);
        return this;
    }

    public IProfileReadOnlyRepository Build() => _mock.Object;
}
