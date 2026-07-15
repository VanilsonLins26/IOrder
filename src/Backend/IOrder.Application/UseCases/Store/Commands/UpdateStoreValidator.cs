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
        RuleFor(store => store.BaseDeliveryFee)
            .InclusiveBetween(0m, 100m).WithMessage("Taxa base deve ser entre R$ 0,00 e R$ 100,00.");
        RuleFor(store => store.FeePerKm)
            .InclusiveBetween(0m, 50m).WithMessage("Valor por km deve ser entre R$ 0,00 e R$ 50,00.");
        RuleFor(store => store.MaxDeliveryDistanceKm)
            .InclusiveBetween(1.0, 100.0).WithMessage("Distância máxima deve ser entre 1 e 100 km.");
    }
}

