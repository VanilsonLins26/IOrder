using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Customization;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Customization;

internal class CustomizationRepository : ICustomizationReadOnlyRepository, ICustomizationWriteOnlyRepository
{
    private readonly AppDbContext _db;

    public CustomizationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomizationGroup>> GetByProductId(Guid productId)
    {
        return await _db.Set<CustomizationGroup>()
            .Include(g => g.Options.OrderBy(o => o.Position))
            .Where(g => g.ProductId == productId)
            .OrderBy(g => g.Position)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveGroup(CustomizationGroup group)
    {
        _db.Set<CustomizationGroup>().Add(group);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteGroup(Guid groupId)
    {
        var group = await _db.Set<CustomizationGroup>().FindAsync(groupId);
        if (group is not null)
        {
            _db.Set<CustomizationGroup>().Remove(group);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteOption(Guid optionId)
    {
        var option = await _db.Set<CustomizationOption>().FindAsync(optionId);
        if (option is not null)
        {
            _db.Set<CustomizationOption>().Remove(option);
            await _db.SaveChangesAsync();
        }
    }
}
