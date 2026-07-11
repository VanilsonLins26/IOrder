using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Security.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


using System.Diagnostics.CodeAnalysis;

namespace IOrder.infrastructure.LoggedUser;

[ExcludeFromCodeCoverage]
internal class LoggedUserService : ILoggedUserService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IProfileReadOnlyRepository _profileRepository;

    public LoggedUserService(IHttpContextAccessor accessor, IProfileReadOnlyRepository profileRepository)
    {
        _accessor = accessor;
        _profileRepository = profileRepository;
    }

    public string GetUserId()
    {
        return _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string GetUserEmail()
    {
        var email = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            email = _accessor.HttpContext?.User?.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                email = _profileRepository.GetByUserId(userId).GetAwaiter().GetResult()?.Email;
            }
        }

        return email;
    }

    public string? GetUserPhone()
    {
        var phone = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.MobilePhone)?.Value;

        if (string.IsNullOrEmpty(phone))
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                phone = _profileRepository.GetByUserId(userId).GetAwaiter().GetResult()?.Phone;
            }
        }

        return phone;
    }

    public bool IsShopkeeper()
    {
        return _accessor.HttpContext?.User?.IsInRole("Shopkeeper") ?? false;
    }

    public bool IsClient()
    {
        return _accessor.HttpContext?.User?.IsInRole("Client") ?? false;
    }
}
