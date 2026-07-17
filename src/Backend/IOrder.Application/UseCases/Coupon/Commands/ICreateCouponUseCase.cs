using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Coupon.Commands;

public interface ICreateCouponUseCase
{
    Task<CouponResponseDto> Execute(Communication.Request.CouponRequestDto request);
}
