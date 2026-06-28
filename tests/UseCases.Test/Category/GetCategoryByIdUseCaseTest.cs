using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class GetCategoryByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);
        
        var useCase = CreateUseCase(storeId, category);

        var response = await useCase.Execute(category.Id);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(category.Name);
        response.Position.ShouldBe(category.Position);
    }

    [Fact]
    public async Task Error_Category_Not_Found()
    {
        var storeId = Guid.NewGuid();
        var useCase = CreateUseCase(storeId);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.CATEGORY_NOT_FOUND);
    }

    private GetCategoryByIdUseCase CreateUseCase(Guid storeId, IOrder.Domain.Entities.Category? category = null)
    {
        var readOnlyRepository = new CategoryReadOnlyRepositoryBuilder();
        if (category is not null)
            readOnlyRepository.GetByIdAsync(category.Id, category);

        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new GetCategoryByIdUseCase(
            readOnlyRepository.Build(),
            permissionService);
    }
}
