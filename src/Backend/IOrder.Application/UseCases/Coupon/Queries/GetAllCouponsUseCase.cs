using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Coupon;
using Mapster;

namespace IOrder.Application.UseCases.Coupon.Queries;

public class GetAllCouponsUseCase : IGetAllCouponsUseCase
{
    private readonly ICouponReadOnlyRepository _readOnlyRepository;

    public GetAllCouponsUseCase(ICouponReadOnlyRepository readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<IList<CouponResponseDto>> Execute()
    {
        var coupons = await _readOnlyRepository.GetAllCouponsAsync();
        return coupons.Adapt<IList<CouponResponseDto>>();
    }
}
