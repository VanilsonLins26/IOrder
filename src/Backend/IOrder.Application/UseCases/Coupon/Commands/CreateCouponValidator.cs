using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Coupon.Commands;

public class CreateCouponValidator : AbstractValidator<CouponRequestDto>
{
    public CreateCouponValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.DiscountType)
            .NotEmpty().WithMessage(ResourceMessagesException.COUPON_INVALID)
            .Must(BeAValidDiscountType).WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0).When(x => x.MaxDiscountAmount.HasValue)
            .WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.MinPurchaseAmount)
            .GreaterThan(0).When(x => x.MinPurchaseAmount.HasValue)
            .WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).When(x => x.ExpiresAt.HasValue)
            .WithMessage(ResourceMessagesException.COUPON_INVALID);

        RuleFor(x => x.MaxUsageCount)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.COUPON_INVALID);
    }

    private static bool BeAValidDiscountType(string discountType)
    {
        return Enum.TryParse<CouponDiscountType>(discountType, ignoreCase: true, out _);
    }
}
