using IOrder.Domain.Security.Services;
using IOrder.Communication.Response.Profile;
using IOrder.Domain.Repositories.Profile;

namespace IOrder.Application.UseCases.Profile.Queries;

public class GetUserAddressesUseCase : IGetUserAddressesUseCase
{
    private readonly ILoggedUserService _loggedUser;
    private readonly IUserAddressReadOnlyRepository _readRepository;

    public GetUserAddressesUseCase(
        ILoggedUserService loggedUser,
        IUserAddressReadOnlyRepository readRepository)
    {
        _loggedUser = loggedUser;
        _readRepository = readRepository;
    }

    public async Task<List<UserAddressResponseDto>> ExecuteAsync()
    {
        
        var userId = _loggedUser.GetUserId();

        var addresses = await _readRepository.GetUserAddressesAsync(userId);

        return addresses.Select(a => new UserAddressResponseDto
        {
            Id = a.Id,
            Name = a.Name,
            ZipCode = a.ZipCode,
            Street = a.Street,
            Number = a.Number,
            Complement = a.Complement,
            Neighborhood = a.Neighborhood,
            City = a.City,
            State = a.State,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            IsDefault = a.IsDefault
        }).ToList();
    }
}
