using IOrder.Domain.Entities;

namespace IOrder.Domain.Repositories.Payment;

public interface IUserCardReadOnlyRepository
{
    Task<List<UserCard>> GetByUserIdAsync(string userId);
    Task<UserCard?> GetByIdAsync(Guid id);
}

public interface IUserCardWriteOnlyRepository
{
    Task CreateAsync(UserCard userCard);
    void Delete(UserCard userCard);
}
