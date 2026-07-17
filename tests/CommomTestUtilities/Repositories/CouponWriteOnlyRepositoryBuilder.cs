using IOrder.Domain.Repositories.Coupon;
using Moq;

namespace CommomTestUtilities.Repositories;

public class CouponWriteOnlyRepositoryBuilder
{
    private readonly Mock<ICouponWriteOnlyRepository> _mock;

    public CouponWriteOnlyRepositoryBuilder()
    {
        _mock = new Mock<ICouponWriteOnlyRepository>();
    }

    public CouponWriteOnlyRepositoryBuilder Create()
    {
        _mock.Setup(r => r.Create(It.IsAny<IOrder.Domain.Entities.Coupon>()))
            .ReturnsAsync((IOrder.Domain.Entities.Coupon coupon) => coupon);
        return this;
    }

    public ICouponWriteOnlyRepository Build() => _mock.Object;
}
