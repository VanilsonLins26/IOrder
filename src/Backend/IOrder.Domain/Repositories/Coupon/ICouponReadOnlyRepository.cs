namespace IOrder.Domain.Repositories.Coupon;

public interface ICouponReadOnlyRepository
{
    Task<Entities.Coupon?> GetByCodeAsync(string code);
    Task<IList<Entities.Coupon>> GetActiveCouponsAsync();
    Task<IList<Entities.Coupon>> GetAllCouponsAsync();
    Task<bool> CodeExistsAsync(string code);
}
