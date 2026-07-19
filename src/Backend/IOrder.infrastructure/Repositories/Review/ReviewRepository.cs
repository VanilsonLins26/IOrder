using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Review;
using Microsoft.EntityFrameworkCore;
using IOrder.infrastructure.DataAccess;

namespace IOrder.infrastructure.Repositories.Review;

internal class ReviewRepository : IReviewReadOnlyRepository, IReviewWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderReview review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task<OrderReview?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OrderId == orderId);
    }

    public async Task<IList<OrderReview>> GetByStoreIdAsync(Guid storeId, int page, int pageSize)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.StoreId == storeId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
