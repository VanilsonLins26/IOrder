using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Coupon.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.Coupon;

public class GetActiveCouponsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var coupons = new List<IOrder.Domain.Entities.Coupon>
        {
            CouponBuilder.Build("PROMO10"),
            CouponBuilder.Build("PROMO20"),
        };

        var useCase = CreateUseCase(coupons);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var useCase = CreateUseCase([]);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private static GetActiveCouponsUseCase CreateUseCase(IList<IOrder.Domain.Entities.Coupon>? coupons = null)
    {
        var readOnlyRepository = new CouponReadOnlyRepositoryBuilder();
        readOnlyRepository.GetActiveCouponsAsync(coupons ?? []);

        return new GetActiveCouponsUseCase(readOnlyRepository.Build());
    }
}
