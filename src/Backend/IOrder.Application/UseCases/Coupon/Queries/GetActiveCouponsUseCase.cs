using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Coupon;
using Mapster;

namespace IOrder.Application.UseCases.Coupon.Queries;

public class GetActiveCouponsUseCase : IGetActiveCouponsUseCase
{
    private readonly ICouponReadOnlyRepository _readOnlyRepository;

    public GetActiveCouponsUseCase(ICouponReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<IList<CouponResponseDto>> Execute()
    {
        var coupons = await _readOnlyRepository.GetActiveCouponsAsync();
        return coupons.Adapt<IList<CouponResponseDto>>();
    }
}
