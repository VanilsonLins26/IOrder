using CommomTestUtilities.Entities;
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

public class UpdateOpeningHourUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var request = UpdateOpeningHourRequestBuilder.Build();
        var useCase = CreateUseCase(store);

        Func<Task> act = async () => await useCase.Execute(request, store.Id);
        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var store = StoreBuilder.Build();
        var request = UpdateOpeningHourRequestBuilder.Build();
        var useCase = CreateUseCase(store: null);

        Func<Task> act = async () => await useCase.Execute(request, Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.STORE_NOT_FOUND);
    }

    [Fact]
    public async Task Error_OpeningHours_Empty()
    {
        var store = StoreBuilder.Build();
        var request = UpdateOpeningHourRequestBuilder.Build();
        request.OpeningHours = []; 
        var useCase = CreateUseCase(store);

        Func<Task> act = async () => await useCase.Execute(request, store.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.OPENING_HOURS_EMPTY);
    }

    private static UpdateOpeningHourUseCase CreateUseCase(IOrder.Domain.Entities.Store? store = null)
    {
        var writeRepositoryBuilder = new StoreWriteOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        var userId = store != null ? store.UserId : "test-user-id";
        var loggedUserService = LoggedUserBuilder.Build(userId);
        var storePermissionService = StorePermissionServiceBuilder.Build();

        if (store != null)
            writeRepositoryBuilder.GetByIdTracking(store);

        return new UpdateOpeningHourUseCase(writeRepositoryBuilder.Build(), unitOfWork, loggedUserService, storePermissionService);
    }
}
