using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class AddressValidator : AbstractValidator<AddressRequestDto>
{
    public AddressValidator()
    {
        RuleFor(address => address.ZipCode).NotEmpty().WithMessage(ResourceMessagesException.ZIP_CODE_EMPTY)
                                           .Length(8, 9).WithMessage(ResourceMessagesException.ZIP_CODE_INVALID);
       
        RuleFor(a => a.Street).NotEmpty().WithMessage(ResourceMessagesException.STREET_EMPTY)
                              .MaximumLength(150).WithMessage(ResourceMessagesException.STREET_INVALID);
       
        RuleFor(a => a.Number).NotEmpty().WithMessage(ResourceMessagesException.NUMBER_EMPTY)
                              .MaximumLength(10).WithMessage(ResourceMessagesException.NUMBER_INVALID);
       
        RuleFor(a => a.Complement).MaximumLength(50).WithMessage(ResourceMessagesException.COMPLEMENT_INVALID);
       
        RuleFor(a => a.Neighborhood).NotEmpty().WithMessage(ResourceMessagesException.NEIGHBORHOOD_EMPTY)
                                    .MaximumLength(100).WithMessage(ResourceMessagesException.NEIGHBORHOOD_INVALID);
       
        RuleFor(a => a.City).NotEmpty().WithMessage(ResourceMessagesException.CITY_EMPTY)
                            .MaximumLength(100).WithMessage(ResourceMessagesException.CITY_INVALID);
        
        RuleFor(a => a.State).NotEmpty().WithMessage(ResourceMessagesException.STATE_EMPTY)
                             .Length(2).WithMessage(ResourceMessagesException.STATE_INVALID);

    }
}
