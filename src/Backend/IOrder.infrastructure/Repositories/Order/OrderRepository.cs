using IOrder.Domain.Repositories.Order;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Order;

internal class OrderRepository : IOrderReadOnlyRepository, IOrderWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IList<Domain.Entities.Order>> GetByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IList<Domain.Entities.Order>> GetByStoreIdAsync(Guid storeId, int pageNumber, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.StoreId == storeId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByUserIdAsync(string userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .CountAsync(o => o.UserId == userId);
    }

    public async Task<int> GetCountByStoreIdAsync(Guid storeId)
    {
        return await _context.Orders
            .AsNoTracking()
            .CountAsync(o => o.StoreId == storeId);
    }

    public async Task<Domain.Entities.Order> Create(Domain.Entities.Order order)
    {
        await _context.Orders.AddAsync(order);
        return order;
    }

    public Domain.Entities.Order Update(Domain.Entities.Order order)
    {
        _context.Orders.Update(order);
        return order;
    }

    public async Task<Domain.Entities.Order?> GetByIdTracking(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Messages)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
