using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Chat.Queries;
using IOrder.Domain.Repositories.Order;
using Shouldly;

namespace UseCases.Test.Chat;

public class GetConversationsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var summaries = new List<ConversationSummary>
        {
            new()
            {
                OrderId = Guid.NewGuid(),
                StoreId = store.Id,
                StoreName = store.Name,
                StoreImageUrl = store.ImageUrl,
                Status = "Pending",
                LastMessage = "Olá",
                LastMessageAt = DateTime.UtcNow,
                LastMessageByRole = "client",
                UnreadCount = 2
            }
        };
        var useCase = CreateUseCase(store, summaries, totalCount: 1);

        var response = await useCase.Execute(pageNumber: 1, pageSize: 20);

        response.ShouldNotBeNull();
        response.Items.ShouldHaveSingleItem();
        response.TotalCount.ShouldBe(1);
        response.Items[0].StoreName.ShouldBe(store.Name);
        response.Items[0].UnreadCount.ShouldBe(2);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store, new List<ConversationSummary>(), totalCount: 0);

        var response = await useCase.Execute(pageNumber: 1, pageSize: 20);

        response.ShouldNotBeNull();
        response.Items.ShouldBeEmpty();
        response.TotalCount.ShouldBe(0);
    }

    private static GetConversationsUseCase CreateUseCase(
        IOrder.Domain.Entities.Store store,
        IList<ConversationSummary> summaries,
        int totalCount)
    {
        var chatRepository = new ChatReadOnlyRepositoryBuilder()
            .GetConversationsAsync(summaries)
            .GetConversationsCountAsync(totalCount)
            .Build();
        var storeReadOnly = new StoreReadOnlyRepositoryBuilder()
            .GetByUserIdAsync(store)
            .Build();
        var loggedUser = LoggedUserBuilder.Build();

        return new GetConversationsUseCase(chatRepository, storeReadOnly, loggedUser);
    }
}
