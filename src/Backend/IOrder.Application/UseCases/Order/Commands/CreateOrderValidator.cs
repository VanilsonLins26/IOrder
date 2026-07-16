using FluentValidation;
using IOrder.Communication.Enums;
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

        RuleFor(x => x.DeliveryType)
            .IsInEnum().WithMessage("Tipo de entrega inválido.");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).When(x => x.DeliveryType == DeliveryTypeDto.Delivery)
            .WithMessage("Taxa de entrega não pode ser negativa.");

        RuleFor(x => x.DeliveryFee)
            .Equal(0).When(x => x.DeliveryType == DeliveryTypeDto.Pickup)
            .WithMessage("Taxa de entrega deve ser zero para retirada.");
    }
}
