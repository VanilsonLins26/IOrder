using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Security.Services;

public interface ILoggedUserService
{
    string GetUserId();
    string GetUserEmail();
    bool IsShopkeeper();
    bool IsClient();
}
