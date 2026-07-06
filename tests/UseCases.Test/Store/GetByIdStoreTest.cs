using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Application.UseCases.Store.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Store;

public class GetByIdStoreTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store);

        var result = await useCase.Execute(store.Id);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(store.Name);
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.STORE_NOT_FOUND);
    }

    private static GetByIdStoreUseCase CreateUseCase(IOrder.Domain.Entities.Store? store = null)
    {
        var readRepositoryBuilder = new StoreReadOnlyRepositoryBuilder();

        if (store != null)
            readRepositoryBuilder.GetByIdAsync(store);

        return new GetByIdStoreUseCase(readRepositoryBuilder.Build());
    }
}
