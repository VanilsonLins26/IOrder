using IOrder.Domain.Repositories.Coupon;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Coupon;

internal class CouponRepository : ICouponReadOnlyRepository, ICouponWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public CouponRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Coupon?> GetByCodeAsync(string code)
    {
        return await _context.Coupons.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IList<Domain.Entities.Coupon>> GetActiveCouponsAsync()
    {
        return await _context.Coupons.AsNoTracking()
            .Where(c => c.Active && (!c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(c => c.DiscountValue)
            .ToListAsync();
    }

    public async Task<IList<Domain.Entities.Coupon>> GetAllCouponsAsync()
    {
        return await _context.Coupons.AsNoTracking()
            .OrderByDescending(c => c.Code)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Coupons.AnyAsync(c => c.Code == code);
    }

    public async Task<Domain.Entities.Coupon> Create(Domain.Entities.Coupon coupon)
    {
        await _context.Coupons.AddAsync(coupon);
        return coupon;
    }

    public Domain.Entities.Coupon Update(Domain.Entities.Coupon coupon)
    {
        _context.Coupons.Update(coupon);
        return coupon;
    }

    public async Task<Domain.Entities.Coupon?> GetByIdTracking(Guid id)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);
    }
}
