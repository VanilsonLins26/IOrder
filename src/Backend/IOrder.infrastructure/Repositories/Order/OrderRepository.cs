using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Order;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Order;

internal class OrderRepository : IOrderReadOnlyRepository, IOrderWriteOnlyRepository, IChatReadOnlyRepository
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
            .Include(o => o.Store)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IList<Domain.Entities.Order>> GetByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .Include(o => o.Messages.OrderByDescending(m => m.SentAt).Take(1))
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
            .Include(o => o.Messages.OrderByDescending(m => m.SentAt).Take(1))
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

    public void AddOrderMessage(OrderMessage message)
    {
        _context.OrderMessages.Add(message);
    }

    public async Task MarkMessagesAsReadAsync(Guid orderId, string readByUserId)
    {
        var now = DateTime.UtcNow;
        var unread = await _context.OrderMessages
            .Where(m => m.OrderId == orderId && m.UserId != readByUserId && m.ReadAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.ReadAt, now)
                .SetProperty(m => m.ReadByUserId, readByUserId));
    }

    public async Task<IList<ConversationSummary>> GetConversationsAsync(
        string userId, string? storeUserId, int pageNumber, int pageSize)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId || o.Store.UserId == userId)
            .Where(o => o.Messages.Any())
            .Select(o => new ConversationSummary
            {
                OrderId = o.Id,
                StoreId = o.StoreId,
                StoreName = o.Store.Name,
                StoreImageUrl = o.Store.ImageUrl,
                Status = o.Status.ToString(),
                LastMessage = o.Messages.OrderByDescending(m => m.SentAt).Select(m => m.Message).FirstOrDefault(),
                LastMessageAt = o.Messages.Max(m => m.SentAt),
                LastMessageByRole = o.Messages.OrderByDescending(m => m.SentAt).Select(m => m.UserRole).FirstOrDefault(),
                UnreadCount = o.Messages.Count(m => m.UserId != userId && m.ReadAt == null)
            })
            .OrderByDescending(c => c.LastMessageAt);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetConversationsCountAsync(string userId, string? storeUserId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId || o.Store.UserId == userId)
            .Where(o => o.Messages.Any())
            .CountAsync();
    }

    public async Task<IList<OrderMessage>> GetMessagesAsync(Guid orderId, int pageNumber, int pageSize)
    {
        return await _context.OrderMessages
            .AsNoTracking()
            .Where(m => m.OrderId == orderId)
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetMessagesCountAsync(Guid orderId)
    {
        return await _context.OrderMessages
            .AsNoTracking()
            .CountAsync(m => m.OrderId == orderId);
    }

    public async Task<int> GetUnreadCountAsync(Guid orderId, string userId)
    {
        return await _context.OrderMessages
            .AsNoTracking()
            .CountAsync(m => m.OrderId == orderId && m.UserId != userId && m.ReadAt == null);
    }
}
