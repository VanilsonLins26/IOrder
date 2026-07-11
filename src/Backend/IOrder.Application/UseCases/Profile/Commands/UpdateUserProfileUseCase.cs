using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;

namespace IOrder.Application.UseCases.Profile.Commands;

public interface IUpdateUserProfileUseCase
{
    Task<UserProfileResponseDto> Execute(UserProfileRequestDto request);
}

public class UpdateUserProfileUseCase : IUpdateUserProfileUseCase
{
    private readonly IProfileReadOnlyRepository _readOnlyRepository;
    private readonly IProfileWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUser;

    public UpdateUserProfileUseCase(
        IProfileReadOnlyRepository readOnlyRepository,
        IProfileWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork uof,
        ILoggedUserService loggedUser)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUser = loggedUser;
    }

    public async Task<UserProfileResponseDto> Execute(UserProfileRequestDto request)
    {
        var userId = _loggedUser.GetUserId();
        var profile = await _readOnlyRepository.GetByUserId(userId);

        if (profile is null)
        {
            profile = new Domain.Entities.UserProfile
            {
                UserId = userId,
                Phone = request.Phone,
                Email = request.Email
            };
            await _writeOnlyRepository.Create(profile);
        }
        else
        {
            profile.Phone = request.Phone;

            if (!string.IsNullOrEmpty(request.Email))
            {
                profile.Email = request.Email;
                profile.EmailManuallySet = true;
            }

            _writeOnlyRepository.Update(profile);
        }

        await _uof.Commit();

        return new UserProfileResponseDto
        {
            Phone = profile.Phone ?? string.Empty,
            Email = profile.Email
        };
    }
}
