using CommomTestUtilities.Requests.Coupon;
using IOrder.Application.UseCases.Coupon.Commands;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Coupon;

public class CreateCouponValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Code_Empty()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.Code = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_DiscountType_Invalid()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.DiscountType = "InvalidType";

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_DiscountValue_Zero()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.DiscountValue = 0;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_MaxDiscountAmount_Negative()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.MaxDiscountAmount = -1;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_MinPurchaseAmount_Negative()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.MinPurchaseAmount = -1;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_ExpiresAt_InPast()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.ExpiresAt = DateTime.UtcNow.AddDays(-1);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }

    [Fact]
    public void Error_MaxUsageCount_Negative()
    {
        var validator = new CreateCouponValidator();
        var request = CouponRequestBuilder.Build();
        request.MaxUsageCount = -1;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.COUPON_INVALID);
    }
}
