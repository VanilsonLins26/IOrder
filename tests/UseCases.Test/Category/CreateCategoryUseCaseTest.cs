using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Category;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Category.Commands;
using IOrder.Application.UseCases.Category.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class CreateCategoryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = CategoryRequestBuilder.Build();
        var storeId = Guid.NewGuid();
        var useCase = CreateUseCase(storeId);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(request.Name);
        response.Position.ShouldBe(request.Position.Value);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = CategoryRequestBuilder.Build();
        request.Name = string.Empty;
        var storeId = Guid.NewGuid();
        var useCase = CreateUseCase(storeId);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var request = CategoryRequestBuilder.Build();
        var storeId = Guid.NewGuid();
        var useCase = CreateUseCase(storeId, request.Name);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_ALREADY_EXISTS);
    }

    private CreateCategoryUseCase CreateUseCase(Guid storeId, string? existingCategoryName = null)
    {
        var readOnlyRepository = new CategoryReadOnlyRepositoryBuilder();
        if (existingCategoryName is not null)
            readOnlyRepository.NameExists(existingCategoryName, storeId, true);

        var writeOnlyRepository = new CategoryWriteOnlyRepositoryBuilder().Create();
        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new CreateCategoryUseCase(
            readOnlyRepository.Build(),
            writeOnlyRepository.Build(),
            uow,
            permissionService);
    }
}
