using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Coupon.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.Coupon;

public class GetAllCouponsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var coupons = new List<IOrder.Domain.Entities.Coupon>
        {
            CouponBuilder.Build("PROMO10"),
            CouponBuilder.Build("PROMO20"),
            CouponBuilder.Build("PROMO30"),
        };

        var useCase = CreateUseCase(coupons);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var useCase = CreateUseCase([]);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private static GetAllCouponsUseCase CreateUseCase(IList<IOrder.Domain.Entities.Coupon>? coupons = null)
    {
        var readOnlyRepository = new CouponReadOnlyRepositoryBuilder();
        readOnlyRepository.GetAllCouponsAsync(coupons ?? []);

        return new GetAllCouponsUseCase(readOnlyRepository.Build());
    }
}
