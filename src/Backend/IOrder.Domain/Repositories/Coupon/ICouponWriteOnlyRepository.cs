namespace IOrder.Domain.Repositories.Coupon;

public interface ICouponWriteOnlyRepository
{
    Task<Entities.Coupon> Create(Entities.Coupon coupon);
    Entities.Coupon Update(Entities.Coupon coupon);
    Task<Entities.Coupon?> GetByIdTracking(Guid id);
}
