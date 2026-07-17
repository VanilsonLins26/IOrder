using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
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

public class DeleteStoreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store);

        Func<Task> act = async () => await useCase.Execute(store.Id);
        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.STORE_NOT_FOUND);
    }

    private static DeleteStoreUseCase CreateUseCase(IOrder.Domain.Entities.Store? store = null)
    {
        var writeRepositoryBuilder = new StoreWriteOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        var userId = store != null ? store.UserId : "test-user-id";
        var loggedUserService = LoggedUserBuilder.Build(userId);
        var storePermissionService = StorePermissionServiceBuilder.Build();

        if (store != null)
        {
            writeRepositoryBuilder.GetByIdTracking(store);
            writeRepositoryBuilder.Delete();
        }

        return new DeleteStoreUseCase(writeRepositoryBuilder.Build(), unitOfWork, loggedUserService, storePermissionService);
    }
}
