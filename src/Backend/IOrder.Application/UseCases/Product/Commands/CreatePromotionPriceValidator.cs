using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product.Commands;

public class CreatePromotionPriceValidator : AbstractValidator<PromotionPriceResquestDto>
{
    public CreatePromotionPriceValidator()
    {
        RuleFor(request => request.ProductId).NotEmpty().WithMessage(ResourceMessagesException.PRODUCT_ID_EMPTY);
        RuleFor(request => request.Price).Cascade(CascadeMode.Stop).NotNull().WithMessage(ResourceMessagesException.PRICE_EMPTY)
                                         .GreaterThan(0).WithMessage(ResourceMessagesException.PRICE_GREATER_THAN_0);
        RuleFor(request => request.InitialTime).Cascade(CascadeMode.Stop).NotNull().WithMessage(ResourceMessagesException.INITIAL_TIME_EMPTY)
                                               .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage(ResourceMessagesException.INITIAL_TIME_LESS_THAN_NOW);
        RuleFor(request => request.FinalTime).Cascade(CascadeMode.Stop).NotNull().WithMessage(ResourceMessagesException.FINAL_TIME_EMPTY);
        When(request => request.InitialTime.HasValue && request.FinalTime.HasValue, () =>
        {
            RuleFor(request => request.FinalTime)
                .GreaterThanOrEqualTo(request => request.InitialTime).WithMessage(ResourceMessagesException.FINAL_TIME_LESS_THAN_INITIAL);
        });
    }

}

