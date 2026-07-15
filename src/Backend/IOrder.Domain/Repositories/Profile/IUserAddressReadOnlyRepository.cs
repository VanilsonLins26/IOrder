using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Profile;

public interface IUserAddressReadOnlyRepository
{
    Task<List<UserAddress>> GetUserAddressesAsync(string userId);
    Task<UserAddress?> GetByIdAsync(Guid id);
    Task<int> CountUserAddressesAsync(string userId);
    Task<UserAddress?> GetDefaultAddressAsync(string userId);
}
