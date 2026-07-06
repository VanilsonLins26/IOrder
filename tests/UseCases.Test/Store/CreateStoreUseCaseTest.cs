using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Store;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Application.UseCases.Store.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Store;

public class CreateStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = StoreRequestBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.About.ShouldBe(request.About);
        result.ImageUrl.ShouldBe(request.ImageUrl);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var request = StoreRequestBuilder.Build();
        var useCase = CreateUseCase(request.Name);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_ALREADY_EXISTS);
    }

    [Fact]
    public async Task Error_User_Already_Has_Store()
    {
        var request = StoreRequestBuilder.Build();
        var useCase = CreateUseCase(userHasStore: true);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.USER_ALREADY_HAS_STORE);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = StoreRequestBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    private static CreateStoreUseCase CreateUseCase(string? storeName = null, bool userHasStore = false)
    {
        var writeRepository = new StoreWriteOnlyRepositoryBuilder().Create().Build();
        var readRepositoryBuilder = new StoreReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        var userId = "test-user-id";
        var loggedUserService = LoggedUserBuilder.Build(userId);

        if (!string.IsNullOrEmpty(storeName))
            readRepositoryBuilder.NameExists(storeName);

        if (userHasStore)
            readRepositoryBuilder.HasStore(userId);

        return new CreateStoreUseCase(readRepositoryBuilder.Build(), writeRepository, unitOfWork, loggedUserService, new IOrder.Application.UseCases.Store.Commands.CreateStoreValidator());
    }
}
