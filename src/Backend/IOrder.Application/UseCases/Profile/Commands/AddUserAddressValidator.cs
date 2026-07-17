using FluentValidation;
using IOrder.Communication.Request.Profile;

namespace IOrder.Application.UseCases.Profile.Commands;

public class AddUserAddressValidator : AbstractValidator<AddUserAddressRequestDto>
{
    public AddUserAddressValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.ZipCode).NotEmpty().WithMessage("ZipCode is required.");
        RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required.");
        RuleFor(x => x.Neighborhood).NotEmpty().WithMessage("Neighborhood is required.");
        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.State).NotEmpty().WithMessage("State is required.");
    }
}
