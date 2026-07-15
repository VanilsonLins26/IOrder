using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Store.Queries;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Store;

public class GetByIdStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        
        var readOnlyBuilder = new StoreReadOnlyRepositoryBuilder().GetByIdAsync(store);
        var useCase = new GetByIdStoreUseCase(readOnlyBuilder.Build());

        var response = await useCase.Execute(store.Id);

        response.ShouldNotBeNull();
        response.Id.ShouldBe(store.Id);
        response.Name.ShouldBe(store.Name);
    }

    [Fact]
    public async Task Error_Store_Not_Found_Should_Throw()
    {
        var readOnlyBuilder = new StoreReadOnlyRepositoryBuilder().GetByIdAsyncReturnsNull();
        var useCase = new GetByIdStoreUseCase(readOnlyBuilder.Build());

        var ex = await Should.ThrowAsync<NotFoundException>(() => useCase.Execute(Guid.NewGuid()));
        ex.GetErrorMessages().ShouldContain(IOrder.Exceptions.ResourceMessagesException.STORE_NOT_FOUND);
    }
}
