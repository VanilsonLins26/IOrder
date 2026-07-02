using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Domain.Entities;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public class OpeningHourValidator : AbstractValidator<OpeningHourRequestDto>
{
    public OpeningHourValidator()
    {
        RuleFor(openingHour => openingHour.DayOfWeek).NotNull().WithMessage(ResourceMessagesException.DAY_OF_WEEK_EMPTY)
                                                     .IsInEnum().WithMessage(ResourceMessagesException.DAY_OF_WEEK_INVALID);
      
    }
}

