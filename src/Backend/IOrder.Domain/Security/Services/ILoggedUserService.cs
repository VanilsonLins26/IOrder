using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Security.Services;

public interface ILoggedUserService
{
    string GetUserId();
    string? GetUserName();
    string GetUserEmail();
    string? GetUserPhone();
    bool IsShopkeeper();
    bool IsClient();
}
