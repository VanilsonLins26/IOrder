using CommomTestUtilities.Requests.Cart;
using Shouldly;
using IOrder.Application.UseCases.Cart.Commands;

namespace Validators.Tests.Cart;

public class ApplyCouponValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new ApplyCouponValidator();
        var request = ApplyCouponRequestBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_Empty_CouponCode(string? couponCode)
    {
        // Arrange
        var validator = new ApplyCouponValidator();
        var request = ApplyCouponRequestBuilder.Build();
        request.CouponCode = couponCode!;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
    }
}
