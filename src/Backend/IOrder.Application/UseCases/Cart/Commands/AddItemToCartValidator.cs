using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Cart.Commands;

public class AddItemToCartValidator :AbstractValidator<AddItemToCartRequestDto>
{
    public AddItemToCartValidator()
    {
        RuleFor(cart => cart.ProductId).NotEmpty().WithMessage(ResourceMessagesException.PRODUCT_ID_EMPTY);
        RuleFor(cart => cart.Quantity).NotNull().WithMessage(ResourceMessagesException.QUANTITY_NULL)
                                      .GreaterThan(0).WithMessage(ResourceMessagesException.QUANTITY_INVALID);
        RuleFor(cart => cart.Customize).MaximumLength(500).WithMessage(ResourceMessagesException.CUSTOMIZE_TOO_LONG);
    }
}
