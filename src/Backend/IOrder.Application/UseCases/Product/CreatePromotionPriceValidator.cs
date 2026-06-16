using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Product;

public class CreatePromotionPriceValidator : AbstractValidator<PromotionPriceResquestDto>
{
    public CreatePromotionPriceValidator()
    {
        RuleFor(request => request.ProductId).NotEmpty().WithMessage(ResourceMessagesException.PRICE_EMPTY);
        RuleFor(request => request.Price).NotEmpty().WithMessage(ResourceMessagesException.PRICE_EMPTY);
        RuleFor(request => request.Price).GreaterThan(0).WithMessage(ResourceMessagesException.PRICE_GREATER_THAN_0);
        RuleFor(request => request.InitialTime).NotEmpty().WithMessage(ResourceMessagesException.INITIAL_TIME_EMPTY);
        RuleFor(request => request.InitialTime).LessThan(DateTime.Now).WithMessage(ResourceMessagesException.INITIAL_TIME_LESS_THAN_NOW);
        RuleFor(request => request.FinalTime).LessThanOrEqualTo(request => request.InitialTime).WithMessage(ResourceMessagesException.FINAL_TIME_LESS_THAN_INITIAL);
        RuleFor(request => request.FinalTime).NotEmpty().WithMessage(ResourceMessagesException.FINAL_TIME_EMPTY);


    }

}
