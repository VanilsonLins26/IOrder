using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IUpdateCourierLocationUseCase
{
    Task Execute(UpdateCourierLocationRequestDto request);
}

public class UpdateCourierLocationUseCase : IUpdateCourierLocationUseCase
{
    private readonly IValidator<UpdateCourierLocationRequestDto> _validator;
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICourierLocationWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourierLocationUseCase(
        IValidator<UpdateCourierLocationRequestDto> validator,
        ILoggedUserService loggedUserService,
        ICourierLocationWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _loggedUserService = loggedUserService;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(UpdateCourierLocationRequestDto request)
    {
        await Validate(request);

        var userId = _loggedUserService.GetUserId();

        var location = new Domain.Entities.CourierLocation
        {
            CourierUserId = userId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            UpdatedAt = DateTime.UtcNow
        };

        await _writeOnlyRepository.UpsertAsync(location);
        await _unitOfWork.Commit();
    }

    private async Task Validate(UpdateCourierLocationRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
