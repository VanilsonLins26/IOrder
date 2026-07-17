using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Order.Commands;

public class SendOrderMessageValidator : AbstractValidator<SendOrderMessageRequestDto>
{
    public SendOrderMessageValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(ResourceMessagesException.ORDER_MESSAGE_EMPTY)
            .MaximumLength(1000).WithMessage(ResourceMessagesException.CUSTOMIZE_TOO_LONG);

        RuleFor(x => x.ProposedTotalAmount)
            .GreaterThan(0).When(x => x.Type == Communication.Enums.MessageTypeDto.Proposal && x.ProposedTotalAmount.HasValue)
            .WithMessage(ResourceMessagesException.PRICE_GREATER_THAN_0);

        RuleFor(x => x.ProposedDeliveryDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.ProposedDeliveryDate.HasValue)
            .WithMessage(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }
}
