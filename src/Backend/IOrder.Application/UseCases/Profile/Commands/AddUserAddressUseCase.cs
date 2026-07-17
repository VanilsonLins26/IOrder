using IOrder.Domain.Security.Services;
using IOrder.Communication.Request.Profile;
using IOrder.Communication.Response.Profile;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Services;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Profile.Commands;

public class AddUserAddressUseCase : IAddUserAddressUseCase
{
    private readonly ILoggedUserService _loggedUser;
    private readonly IUserAddressReadOnlyRepository _readRepository;
    private readonly IUserAddressWriteOnlyRepository _writeRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserAddressUseCase(
        ILoggedUserService loggedUser,
        IUserAddressReadOnlyRepository readRepository,
        IUserAddressWriteOnlyRepository writeRepository,
        IGeocodingService geocodingService,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _geocodingService = geocodingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserAddressResponseDto> ExecuteAsync(AddUserAddressRequestDto request)
    {
        var validator = new AddUserAddressValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }

        
        var userId = _loggedUser.GetUserId();

        var addressCount = await _readRepository.CountUserAddressesAsync(userId);
        if (addressCount >= 10)
        {
            throw new ErrorOnValidationException(new List<string> { "Maximum of 10 addresses allowed." });
        }

        var isDefault = addressCount == 0; // First address is default

        var coords = await _geocodingService.GetCoordinatesAsync(request.Street, request.City, request.State, request.ZipCode);
        double lat = coords?.Latitude ?? 0;
        double lon = coords?.Longitude ?? 0;

        var userAddress = new UserAddress
        {
            UserId = userId,
            Name = request.Name,
            ZipCode = request.ZipCode,
            Street = request.Street,
            Number = request.Number,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            City = request.City,
            State = request.State,
            Latitude = lat,
            Longitude = lon,
            Location = coords.HasValue ? new NetTopologySuite.Geometries.Point(lat, lon) { SRID = 4326 } : null,
            IsDefault = isDefault
        };

        await _writeRepository.AddAsync(userAddress);
        await _unitOfWork.Commit();

        return new UserAddressResponseDto
        {
            Id = userAddress.Id,
            Name = userAddress.Name,
            ZipCode = userAddress.ZipCode,
            Street = userAddress.Street,
            Number = userAddress.Number,
            Complement = userAddress.Complement,
            Neighborhood = userAddress.Neighborhood,
            City = userAddress.City,
            State = userAddress.State,
            Latitude = userAddress.Latitude,
            Longitude = userAddress.Longitude,
            IsDefault = userAddress.IsDefault
        };
    }
}
