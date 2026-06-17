using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Product;

public class CreateProductValidator : AbstractValidator<ProductRequestDto>
{
    public CreateProductValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(request => request.Price).NotNull().WithMessage(ResourceMessagesException.PRICE_EMPTY)
                                         .GreaterThan(0).WithMessage(ResourceMessagesException.PRICE_GREATER_THAN_0);
        RuleFor(request => request.UnitOfMeasure).NotEmpty().WithMessage(ResourceMessagesException.UNIT_OF_MEASURE_EMPTY);

    }
}
