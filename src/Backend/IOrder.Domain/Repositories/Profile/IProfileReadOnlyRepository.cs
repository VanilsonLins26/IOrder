using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Profile;

public interface IProfileReadOnlyRepository
{
    Task<UserProfile?> GetByUserId(string userId);
}
