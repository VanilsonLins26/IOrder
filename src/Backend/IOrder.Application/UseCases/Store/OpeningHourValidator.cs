using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Domain.Entities;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class OpeningHourValidator : AbstractValidator<OpeningHourRequestDto>
{
    public OpeningHourValidator()
    {
        RuleFor(openingHour => openingHour.DayOfWeek).NotNull().WithMessage(ResourceMessagesException.DAY_OF_WEEK_EMPTY)
                                                     .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.DAY_OF_WEEK_INVALID)
                                                     .LessThan(7).WithMessage(ResourceMessagesException.DAY_OF_WEEK_INVALID);
      
    }
}
