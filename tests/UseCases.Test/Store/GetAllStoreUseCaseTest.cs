using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Store;
using IOrder.Application.UseCases.Store.Queries;
using Shouldly;
using Xunit;

namespace UseCases.Test.Store;

public class GetAllStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = StoreSearchRequestBuilder.Build();
        var stores = StoreBuilder.BuildList(3);
        
        var readOnlyBuilder = new StoreReadOnlyRepositoryBuilder().GetAllPaged(stores);
        var useCase = new GetAllStoreUseCase(readOnlyBuilder.Build());

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Items.ShouldNotBeNull();
        response.Items.Count.ShouldBe(stores.Count);
        response.TotalCount.ShouldBe(stores.Count);
        response.CurrentPage.ShouldBe(request.PageNumber);
        response.PageSize.ShouldBe(request.PageSize);
    }
}
