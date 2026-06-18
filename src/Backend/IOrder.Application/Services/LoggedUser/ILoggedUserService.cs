using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.Services.LoggedUser;

public interface ILoggedUserService
{
    string GetUserId();
    string GetUserEmail();
    bool IsShopkeeper();
}
