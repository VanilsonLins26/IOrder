using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Profile;

public interface IUserAddressWriteOnlyRepository
{
    Task AddAsync(UserAddress address);
    Task UpdateAsync(UserAddress address);
    Task DeleteAsync(Guid id);
}
