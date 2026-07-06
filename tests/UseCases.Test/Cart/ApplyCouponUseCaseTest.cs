using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Exceptions.ExceptionBase;
using IOrder.Exceptions;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Cart;

public class ApplyCouponUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = ApplyCouponRequestBuilder.Build();
        var useCase = CreateUseCase();

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.CouponCode.ShouldBe(request.CouponCode);
    }

    [Fact]
    public async Task Error_Empty_Coupon()
    {
        var request = ApplyCouponRequestBuilder.Build();
        request.CouponCode = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
    }

    private ApplyCouponUseCase CreateUseCase()
    {
        var cart = CartBuilder.Build();
        var readOnlyRepository = new CartReadOnlyRepositoryBuilder().GetCartAsync(cart).Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        var loggedUserService = LoggedUserBuilder.Build(cart.UserId);

        return new ApplyCouponUseCase(
            readOnlyRepository,
            writeOnlyRepository,
            loggedUserService);
    }
}
