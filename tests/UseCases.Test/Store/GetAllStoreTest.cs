using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Application.UseCases.Store.Queries;
using IOrder.Domain.SeedWork.Pagination;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Store;

public class GetAllStoreTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store);

        var query = new StoreSearchQuery { PageNumber = 1, PageSize = 10 };
        var result = await useCase.Execute(query);

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldContain(s => s.Name == store.Name);
    }

    private static GetAllStore CreateUseCase(IOrder.Domain.Entities.Store store)
    {
        var readRepositoryBuilder = new StoreReadOnlyRepositoryBuilder();
        readRepositoryBuilder.GetAllPaged([store]);

        return new GetAllStore(readRepositoryBuilder.Build());
    }
}
