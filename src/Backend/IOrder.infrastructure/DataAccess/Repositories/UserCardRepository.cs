using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Payment;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.DataAccess.Repositories;

internal class UserCardRepository : IUserCardReadOnlyRepository, IUserCardWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public UserCardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(UserCard userCard)
    {
        await _context.UserCards.AddAsync(userCard);
    }

    public void Delete(UserCard userCard)
    {
        _context.UserCards.Remove(userCard);
    }

    public async Task<UserCard?> GetByIdAsync(Guid id)
    {
        return await _context.UserCards.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<UserCard>> GetByUserIdAsync(string userId)
    {
        return await _context.UserCards
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }
}
