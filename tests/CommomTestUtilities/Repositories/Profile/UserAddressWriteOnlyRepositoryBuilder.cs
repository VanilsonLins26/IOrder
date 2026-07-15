using IOrder.Domain.Repositories.Profile;
using Moq;

namespace CommomTestUtilities.Repositories.Profile;

public class UserAddressWriteOnlyRepositoryBuilder
{
    private readonly Mock<IUserAddressWriteOnlyRepository> _repository;

    public UserAddressWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserAddressWriteOnlyRepository>();
    }

    public IUserAddressWriteOnlyRepository Build()
    {
        return _repository.Object;
    }
}
