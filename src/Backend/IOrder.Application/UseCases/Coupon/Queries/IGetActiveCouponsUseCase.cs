using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Coupon.Queries;

public interface IGetActiveCouponsUseCase
{
    Task<IList<CouponResponseDto>> Execute();
}
