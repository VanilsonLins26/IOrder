using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using Moq;

namespace CommomTestUtilities.Repositories;

public class ProfileWriteOnlyRepositoryBuilder
{
    private readonly Mock<IProfileWriteOnlyRepository> _mock;

    public ProfileWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IProfileWriteOnlyRepository>();
    }

    public ProfileWriteOnlyRepositoryBuilder GetByUserIdTracking(UserProfile? profile)
    {
        _mock.Setup(r => r.GetByUserIdTracking(It.IsAny<string>())).ReturnsAsync(profile);
        return this;
    }

    public IProfileWriteOnlyRepository Build() => _mock.Object;
}
