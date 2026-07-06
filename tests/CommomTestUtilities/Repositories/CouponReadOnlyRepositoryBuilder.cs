using IOrder.Domain.Repositories.Coupon;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CouponReadOnlyRepositoryBuilder
{
    private readonly Mock<ICouponReadOnlyRepository> _mock;

    public CouponReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<ICouponReadOnlyRepository>();
    }

    public CouponReadOnlyRepositoryBuilder GetByCodeAsync(IOrder.Domain.Entities.Coupon? coupon)
    {
        _mock.Setup(r => r.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync(coupon);
        return this;
    }

    public void CodeExistsAsync(bool exists)
    {
        _mock.Setup(r => r.CodeExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);
    }

    public void GetActiveCouponsAsync(IList<IOrder.Domain.Entities.Coupon> coupons)
    {
        _mock.Setup(r => r.GetActiveCouponsAsync()).ReturnsAsync(coupons);
    }

    public void GetAllCouponsAsync(IList<IOrder.Domain.Entities.Coupon> coupons)
    {
        _mock.Setup(r => r.GetAllCouponsAsync()).ReturnsAsync(coupons);
    }

    public ICouponReadOnlyRepository Build() => _mock.Object;
}
