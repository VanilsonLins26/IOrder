using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Category;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Category.Commands;
using IOrder.Application.UseCases.Category.Queries;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class UpdateCategoryPositionsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var category1 = CategoryBuilder.Build(storeId);
        var category2 = CategoryBuilder.Build(storeId);
        
        var request = UpdateCategoryPositionsRequestBuilder.Build(2);
        request.Positions[0].CategoryId = category1.Id;
        request.Positions[1].CategoryId = category2.Id;

        var useCase = CreateUseCase(storeId, category1, category2);

        await useCase.Execute(request);

        category1.Position.ShouldBe(request.Positions[0].Position);
        category2.Position.ShouldBe(request.Positions[1].Position);
    }

    private UpdateCategoryPositionsUseCase CreateUseCase(
        Guid storeId, 
        IOrder.Domain.Entities.Category category1, 
        IOrder.Domain.Entities.Category category2)
    {
        var writeOnlyRepository = new CategoryWriteOnlyRepositoryBuilder();
        writeOnlyRepository.GetByIdTracking(category1.Id, category1);
        writeOnlyRepository.GetByIdTracking(category2.Id, category2);

        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new UpdateCategoryPositionsUseCase(
            writeOnlyRepository.Build(),
            permissionService,
            uow);
    }
}
