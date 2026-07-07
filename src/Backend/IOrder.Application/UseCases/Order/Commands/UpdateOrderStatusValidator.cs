using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Order.Commands;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequestDto>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage(ResourceMessagesException.ORDER_INVALID_STATUS);
    }
}
