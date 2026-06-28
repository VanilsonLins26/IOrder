using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Category;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class GetCategoriesByStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var categories = new List<IOrder.Domain.Entities.Category>
        {
            CategoryBuilder.Build(storeId),
            CategoryBuilder.Build(storeId)
        };
        
        // Ensure some order for testing if necessary
        categories[0].Position = 1;
        categories[1].Position = 0;

        var useCase = CreateUseCase(storeId, categories);

        var response = await useCase.Execute(storeId);

        response.ShouldNotBeNull();
        response.Count.ShouldBe(2);
        // The use case orders by Position
        response[0].Name.ShouldBe(categories[1].Name);
        response[1].Name.ShouldBe(categories[0].Name);
    }

    private GetCategoriesByStoreUseCase CreateUseCase(Guid storeId, List<IOrder.Domain.Entities.Category> categories)
    {
        var readOnlyRepository = new CategoryReadOnlyRepositoryBuilder()
            .GetAll(storeId, categories)
            .Build();

        return new GetCategoriesByStoreUseCase(readOnlyRepository);
    }
}
