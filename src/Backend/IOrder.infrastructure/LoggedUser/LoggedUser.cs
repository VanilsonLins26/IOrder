using IOrder.Application.Services.LoggedUser;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace IOrder.infrastructure.LoggedUser;

internal class LoggedUserService : ILoggedUserService
{
    private readonly IHttpContextAccessor _accessor;

    public LoggedUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string GetUserId()
    {
        return _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string GetUserEmail()
    {
        return _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public bool IsShopkeeper()
    {
        return _accessor.HttpContext?.User?.IsInRole("Shopkeeper") ?? false;
    }
}