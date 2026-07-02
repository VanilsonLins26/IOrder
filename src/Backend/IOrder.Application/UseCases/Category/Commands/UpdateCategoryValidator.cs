using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Category.Commands;

public class UpdateCategoryValidator : AbstractValidator<CategoryRequestDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(category => category.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(category => category.Position).NotNull().WithMessage(ResourceMessagesException.POSITION_NULL)
                                              .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.INVALID_POSITION);
    }
}

