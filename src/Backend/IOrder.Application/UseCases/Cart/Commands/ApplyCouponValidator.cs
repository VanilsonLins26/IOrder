using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ApplyCouponValidator : AbstractValidator<ApplyCouponRequestDto>
{
    public ApplyCouponValidator()
    {
        RuleFor(request => request.CouponCode).NotEmpty();
    }
}
