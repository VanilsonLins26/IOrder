using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Chat.Queries;
using IOrder.Domain.Entities;
using Shouldly;

namespace UseCases.Test.Chat;

public class GetMessagesUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var orderId = Guid.NewGuid();
        var messages = new List<OrderMessage>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                UserId = "user-1",
                Message = "Olá",
                UserRole = "client",
                SentAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                UserId = "user-2",
                Message = "Olá, tudo bem?",
                UserRole = "shopkeeper",
                SentAt = DateTime.UtcNow
            }
        };
        var useCase = CreateUseCase(messages, totalCount: 2);

        var response = await useCase.Execute(orderId, pageNumber: 1, pageSize: 50);

        response.ShouldNotBeNull();
        response.Items.Count.ShouldBe(2);
        response.TotalCount.ShouldBe(2);
        response.Items[0].Message.ShouldBe("Olá");
    }

    [Fact]
    public async Task Success_Empty()
    {
        var useCase = CreateUseCase(new List<OrderMessage>(), totalCount: 0);

        var response = await useCase.Execute(Guid.NewGuid(), pageNumber: 1, pageSize: 50);

        response.ShouldNotBeNull();
        response.Items.ShouldBeEmpty();
        response.TotalCount.ShouldBe(0);
    }

    private static GetMessagesUseCase CreateUseCase(
        IList<OrderMessage> messages,
        int totalCount)
    {
        var chatRepository = new ChatReadOnlyRepositoryBuilder()
            .GetMessagesAsync(messages)
            .GetMessagesCountAsync(totalCount)
            .Build();

        return new GetMessagesUseCase(chatRepository);
    }
}
