using FluentValidation;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Payment.Commands;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ResourceMessagesException.PAYMENT_ORDER_ID_EMPTY);

        RuleFor(x => x.Method)
            .IsInEnum().WithMessage(ResourceMessagesException.PAYMENT_METHOD_INVALID);

        RuleFor(x => x.PayerEmail)
            .NotEmpty().WithMessage(ResourceMessagesException.PAYMENT_EMAIL_EMPTY)
            .EmailAddress().WithMessage(ResourceMessagesException.PAYMENT_EMAIL_INVALID);

        RuleFor(x => x.CardToken)
            .NotEmpty().WithMessage(ResourceMessagesException.PAYMENT_CARD_TOKEN_EMPTY)
            .When(x => x.Method == PaymentMethodDto.CreditCard);

        RuleFor(x => x.Installments)
            .NotNull().WithMessage(ResourceMessagesException.PAYMENT_INSTALLMENTS_EMPTY)
            .GreaterThan(0).WithMessage(ResourceMessagesException.PAYMENT_INSTALLMENTS_INVALID)
            .When(x => x.Method == PaymentMethodDto.CreditCard);
    }
}
