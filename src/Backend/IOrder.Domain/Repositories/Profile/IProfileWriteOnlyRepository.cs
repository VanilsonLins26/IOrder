using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Profile;

public interface IProfileWriteOnlyRepository
{
    Task Create(UserProfile profile);
    Task<UserProfile?> GetByUserIdTracking(string userId);
    UserProfile Update(UserProfile profile);
}
