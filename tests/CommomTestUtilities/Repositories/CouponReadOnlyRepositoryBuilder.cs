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

    public ICouponReadOnlyRepository Build() => _mock.Object;
}
