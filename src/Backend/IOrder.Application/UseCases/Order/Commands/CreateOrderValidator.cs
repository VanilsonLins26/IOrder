using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Order.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerNotes)
            .MaximumLength(1000).WithMessage(ResourceMessagesException.CUSTOMIZE_TOO_LONG);

        RuleFor(x => x.DeliveryDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.DeliveryDate.HasValue)
            .WithMessage(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }
}
