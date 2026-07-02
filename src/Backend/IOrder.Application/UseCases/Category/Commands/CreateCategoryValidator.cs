using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Category.Commands;

public class CreateCategoryValidator : AbstractValidator<CategoryRequestDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(category => category.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(category => category.Position).NotNull().WithMessage(ResourceMessagesException.POSITION_NULL)
                                              .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.INVALID_POSITION);
    }
}


