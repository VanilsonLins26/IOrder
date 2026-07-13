using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Payment.Commands;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ResourceMessagesException.PAYMENT_ORDER_ID_EMPTY);
    }
}
