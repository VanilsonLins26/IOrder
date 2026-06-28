using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.StoreCategory;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.StoreCategory;

public class GetAllStoreCategoryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var category1 = StoreCategoryBuilder.Build();
        var category2 = StoreCategoryBuilder.Build();
        var categories = new List<IOrder.Domain.Entities.StoreCategory> { category1, category2 };

        var useCase = CreateUseCase(categories);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Count.ShouldBe(2);
        
        response[0].Name.ShouldBe(category1.Name);
        response[1].Name.ShouldBe(category2.Name);
    }

    [Fact]
    public async Task Success_Empty_List()
    {
        var categories = new List<IOrder.Domain.Entities.StoreCategory>();

        var useCase = CreateUseCase(categories);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.ShouldBeEmpty();
    }

    private GetAllStoreCategoryUseCase CreateUseCase(IList<IOrder.Domain.Entities.StoreCategory> categories)
    {
        var readOnlyRepository = new StoreCategoryReadOnlyRepositoryBuilder()
            .GetAllActive(categories)
            .Build();

        return new GetAllStoreCategoryUseCase(readOnlyRepository);
    }
}
