using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Exceptions;

namespace IOrder.Application.UseCases.Delivery.Commands;

public class AssignCourierValidator : AbstractValidator<AssignCourierRequestDto>
{
    public AssignCourierValidator()
    {
        RuleFor(x => x.CourierUserId)
            .NotEmpty().WithMessage("ID do entregador é obrigatório.");
    }
}

public class UpdateCourierLocationValidator : AbstractValidator<UpdateCourierLocationRequestDto>
{
    public UpdateCourierLocationValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.");
    }
}
