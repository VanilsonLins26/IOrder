using IOrder.Domain.Repositories.StoreCategory;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOrder.infrastructure.Repositories.StoreCategory;

internal class StoreCategoryRepository : IStoreCategoryReadOnlyRepository
{
    private readonly AppDbContext _context;

    public StoreCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Domain.Entities.StoreCategory>> GetAllActive()
    {
        return await _context.StoreCategories.AsNoTracking().ToListAsync();
    }
}
