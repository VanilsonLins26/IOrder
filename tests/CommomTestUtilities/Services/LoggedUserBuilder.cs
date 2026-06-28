using IOrder.Application.Services.LoggedUser;
using Moq;

namespace CommomTestUtilities.Services;

public class LoggedUserBuilder
{
    public static ILoggedUserService Build(string userId = "test-user-id")
    {
        var mock = new Mock<ILoggedUserService>();
        mock.Setup(x => x.GetUserId()).Returns(userId);
        return mock.Object;
    }
}
