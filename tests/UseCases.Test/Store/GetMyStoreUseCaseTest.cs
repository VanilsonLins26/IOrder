using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Store;

public class GetMyStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Name.ShouldBe(store.Name);
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var useCase = CreateUseCase(store: null);

        Func<Task> act = async () => await useCase.Execute();

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.STORE_NOT_FOUND);
    }

    private static GetMyStoreUseCase CreateUseCase(IOrder.Domain.Entities.Store? store = null)
    {
        var readRepositoryBuilder = new StoreReadOnlyRepositoryBuilder();
        var userId = store != null ? store.UserId : "test-user-id";
        var loggedUserService = LoggedUserBuilder.Build(userId);

        if (store != null)
            readRepositoryBuilder.GetByUserIdAsync(store);

        return new GetMyStoreUseCase(readRepositoryBuilder.Build(), loggedUserService);
    }
}
