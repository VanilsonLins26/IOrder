using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class UpdateOpeningHourValidator : AbstractValidator<UpdateOpeningHourRequestDto>
{
    public UpdateOpeningHourValidator()
    {
        RuleFor(store => store.OpeningHours).NotEmpty().WithMessage(ResourceMessagesException.OPENING_HOURS_EMPTY)
                                            .Must(hours => hours.Select(h => h.DayOfWeek).Distinct().Count() == hours.Count).WithMessage(ResourceMessagesException.DUPLICATED_DAY_OF_WEEK);
        RuleForEach(s => s.OpeningHours).SetValidator(new OpeningHourValidator());
    }
}
