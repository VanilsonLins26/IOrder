using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public class UpdateStoreValidator : AbstractValidator<UpdateStoreRequestDto>
{
    public UpdateStoreValidator()
    {
        RuleFor(store => store.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(store => store.About).NotEmpty().WithMessage(ResourceMessagesException.ABOUT_EMPTY);
        RuleFor(store => store.ImageUrl).NotEmpty().WithMessage(ResourceMessagesException.IMAGE_URL_EMPTY);
 
    }
}

