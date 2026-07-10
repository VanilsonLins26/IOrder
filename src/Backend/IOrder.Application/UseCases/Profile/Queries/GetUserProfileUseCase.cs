using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.Profile.Queries;

public interface IGetUserProfileUseCase
{
    Task<UserProfileResponseDto> Execute();
}

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly IProfileReadOnlyRepository _repository;
    private readonly ILoggedUserService _loggedUser;

    public GetUserProfileUseCase(IProfileReadOnlyRepository repository, ILoggedUserService loggedUser)
    {
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task<UserProfileResponseDto> Execute()
    {
        var userId = _loggedUser.GetUserId();
        var profile = await _repository.GetByUserId(userId);

        return new UserProfileResponseDto
        {
            Phone = profile?.Phone ?? string.Empty
        };
    }
}
