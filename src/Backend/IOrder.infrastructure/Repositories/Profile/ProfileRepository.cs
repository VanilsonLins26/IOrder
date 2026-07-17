using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Profile;

internal class ProfileRepository : IProfileReadOnlyRepository, IProfileWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByUserId(string userId)
    {
        return await _context.UserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserProfile?> GetByUserIdTracking(string userId)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task Create(UserProfile profile)
    {
        await _context.UserProfiles.AddAsync(profile);
    }

    public UserProfile Update(UserProfile profile)
    {
        _context.UserProfiles.Update(profile);
        return profile;
    }
}
