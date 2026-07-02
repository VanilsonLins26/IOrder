using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public class CreateStoreValidator : AbstractValidator<StoreRequestDto>
{
    public CreateStoreValidator()
    {
        RuleFor(store => store.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(store => store.Address).NotEmpty().WithMessage(ResourceMessagesException.ADDRESS_EMPTY);
        RuleFor(s => s.Address).SetValidator(new AddressValidator());
        RuleFor(store => store.About).NotEmpty().WithMessage(ResourceMessagesException.ABOUT_EMPTY);
        RuleFor(store => store.ImageUrl).NotEmpty().WithMessage(ResourceMessagesException.IMAGE_URL_EMPTY);
        RuleFor(store => store.OpeningHours).NotEmpty().WithMessage(ResourceMessagesException.OPENING_HOURS_EMPTY)
                                            .Must(hours => hours.Select(h => h.DayOfWeek).Distinct().Count() == hours.Count).WithMessage(ResourceMessagesException.DUPLICATED_DAY_OF_WEEK);
        RuleForEach(s => s.OpeningHours).SetValidator(new OpeningHourValidator());
    }
}


