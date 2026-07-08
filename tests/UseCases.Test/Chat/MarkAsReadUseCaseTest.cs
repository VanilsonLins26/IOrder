using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Chat.Commands;
using Shouldly;

namespace UseCases.Test.Chat;

public class MarkAsReadUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var useCase = CreateUseCase(unreadCountAfter: 0);

        var result = await useCase.Execute(Guid.NewGuid());

        result.ShouldBe(0);
    }

    [Fact]
    public async Task Success_With_Remaining_Unread()
    {
        var useCase = CreateUseCase(unreadCountAfter: 3);

        var result = await useCase.Execute(Guid.NewGuid());

        result.ShouldBe(3);
    }

    private static MarkAsReadUseCase CreateUseCase(int unreadCountAfter)
    {
        var writeOnly = new OrderWriteOnlyRepositoryBuilder().Build();
        var chatRepository = new ChatReadOnlyRepositoryBuilder()
            .GetUnreadCountAsync(unreadCountAfter)
            .Build();
        var loggedUser = LoggedUserBuilder.Build();

        return new MarkAsReadUseCase(writeOnly, chatRepository, loggedUser);
    }
}
