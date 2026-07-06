using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Coupon.Queries;

public interface IGetAllCouponsUseCase
{
    Task<IList<CouponResponseDto>> Execute();
}
