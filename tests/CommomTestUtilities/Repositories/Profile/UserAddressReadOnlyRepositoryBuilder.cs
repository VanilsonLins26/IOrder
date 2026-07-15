using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using Moq;

namespace CommomTestUtilities.Repositories.Profile;

public class UserAddressReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserAddressReadOnlyRepository> _repository;

    public UserAddressReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserAddressReadOnlyRepository>();
    }

    public UserAddressReadOnlyRepositoryBuilder CountUserAddressesAsync(string userId, int count)
    {
        _repository.Setup(repository => repository.CountUserAddressesAsync(userId)).ReturnsAsync(count);
        return this;
    }

    public UserAddressReadOnlyRepositoryBuilder GetByIdAsync(Guid id, UserAddress? address)
    {
        _repository.Setup(repository => repository.GetByIdAsync(id)).ReturnsAsync(address);
        return this;
    }

    public UserAddressReadOnlyRepositoryBuilder GetUserAddressesAsync(string userId, List<UserAddress> addresses)
    {
        _repository.Setup(repository => repository.GetUserAddressesAsync(userId)).ReturnsAsync(addresses);
        return this;
    }

    public IUserAddressReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}
