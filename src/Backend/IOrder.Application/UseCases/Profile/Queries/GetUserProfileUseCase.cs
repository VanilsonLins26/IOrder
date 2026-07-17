using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.Profile.Queries;

public interface IGetUserProfileUseCase
{
    Task<UserProfileResponseDto> Execute();
}

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly IProfileReadOnlyRepository _readOnlyRepository;
    private readonly IProfileWriteOnlyRepository _writeOnlyRepository;
    private readonly ILoggedUserService _loggedUser;
    private readonly IUnitOfWork _uof;

    public GetUserProfileUseCase(
        IProfileReadOnlyRepository readOnlyRepository,
        IProfileWriteOnlyRepository writeOnlyRepository,
        ILoggedUserService loggedUser,
        IUnitOfWork uof)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _loggedUser = loggedUser;
        _uof = uof;
    }

    public async Task<UserProfileResponseDto> Execute()
    {
        var userId = _loggedUser.GetUserId();
        var profile = await _readOnlyRepository.GetByUserId(userId);

        var jwtEmail = _loggedUser.GetUserEmail();
        bool needsCommit = false;

        if (profile is null)
        {
            profile = new Domain.Entities.UserProfile
            {
                UserId = userId,
                Email = jwtEmail
            };
            await _writeOnlyRepository.Create(profile);
            needsCommit = true;
        }
        else if (!profile.EmailManuallySet && !string.IsNullOrEmpty(jwtEmail) && profile.Email != jwtEmail)
        {
            profile.Email = jwtEmail;
            _writeOnlyRepository.Update(profile);
            needsCommit = true;
        }

        if (needsCommit)
            await _uof.Commit();

        return new UserProfileResponseDto
        {
            Phone = profile.Phone ?? string.Empty,
            Email = profile.Email
        };
    }
}
