using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Category;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class UpdateCategoryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = CategoryRequestBuilder.Build();
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);
        
        var useCase = CreateUseCase(storeId, category);

        var response = await useCase.Execute(category.Id, request);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(request.Name);
        response.Position.ShouldBe(request.Position.Value);
    }

    [Fact]
    public async Task Error_Category_Not_Found()
    {
        var request = CategoryRequestBuilder.Build();
        var storeId = Guid.NewGuid();
        var useCase = CreateUseCase(storeId);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.CATEGORY_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var request = CategoryRequestBuilder.Build();
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);
        
        var useCase = CreateUseCase(storeId, category, request.Name);

        Func<Task> act = async () => await useCase.Execute(category.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_ALREADY_EXISTS);
    }

    private UpdateCategoryUseCase CreateUseCase(Guid storeId, IOrder.Domain.Entities.Category? category = null, string? existingCategoryName = null)
    {
        var readOnlyRepository = new CategoryReadOnlyRepositoryBuilder();
        if (existingCategoryName is not null)
            readOnlyRepository.NameExists(existingCategoryName, storeId, true);

        var writeOnlyRepository = new CategoryWriteOnlyRepositoryBuilder();
        if (category is not null)
            writeOnlyRepository.GetByIdTracking(category.Id, category);

        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new UpdateCategoryUseCase(
            readOnlyRepository.Build(),
            writeOnlyRepository.Build(),
            uow,
            permissionService);
    }
}
