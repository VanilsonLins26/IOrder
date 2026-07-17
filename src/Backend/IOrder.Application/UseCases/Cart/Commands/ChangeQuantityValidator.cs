using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ChangeQuantityValidator : AbstractValidator<ChangeCartItemQuantityRequestDto>
{
    public ChangeQuantityValidator()
    {
        RuleFor(request => request.CartItemId)
            .NotEmpty()
            .WithMessage(ResourceMessagesException.CART_ITEM_ID_EMPTY);

        RuleFor(request => request.NewQuantity)
            .GreaterThan(0)
            .WithMessage(ResourceMessagesException.QUANTITY_INVALID);
    }
}
