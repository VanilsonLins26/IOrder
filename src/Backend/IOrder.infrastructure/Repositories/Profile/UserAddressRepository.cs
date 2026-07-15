using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Profile;

internal class UserAddressRepository : IUserAddressReadOnlyRepository, IUserAddressWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public UserAddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserAddress address)
    {
        await _context.UserAddresses.AddAsync(address);
    }

    public async Task<int> CountUserAddressesAsync(string userId)
    {
        return await _context.UserAddresses.CountAsync(a => a.UserId == userId);
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await _context.UserAddresses.FindAsync(id);
        if (address != null)
        {
            _context.UserAddresses.Remove(address);
        }
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id)
    {
        return await _context.UserAddresses.FindAsync(id);
    }

    public async Task<UserAddress?> GetDefaultAddressAsync(string userId)
    {
        return await _context.UserAddresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
    }

    public async Task<List<UserAddress>> GetUserAddressesAsync(string userId)
    {
        return await _context.UserAddresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();
    }

    public Task UpdateAsync(UserAddress address)
    {
        _context.UserAddresses.Update(address);
        return Task.CompletedTask;
    }
}
