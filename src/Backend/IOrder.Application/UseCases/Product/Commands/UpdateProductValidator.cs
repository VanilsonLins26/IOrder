using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product.Commands;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequestDto>
{
    public UpdateProductValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(request => request.Price).NotEmpty().WithMessage(ResourceMessagesException.PRICE_EMPTY);
        RuleFor(request => request.Price).GreaterThan(0).WithMessage(ResourceMessagesException.PRICE_GREATER_THAN_0);
  

    }
}

