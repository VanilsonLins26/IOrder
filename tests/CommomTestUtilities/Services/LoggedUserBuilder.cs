using IOrder.Domain.Security.Services;
using Moq;

namespace CommomTestUtilities.Services;

public class LoggedUserBuilder
{
    public static ILoggedUserService Build(string userId = "test-user-id", string email = "test@test.com")
    {
        var mock = new Mock<ILoggedUserService>();
        mock.Setup(x => x.GetUserId()).Returns(userId);
        mock.Setup(x => x.GetUserEmail()).Returns(email);
        return mock.Object;
    }
}
