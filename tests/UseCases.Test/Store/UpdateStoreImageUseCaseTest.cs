using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Store.Commands;
using Shouldly;

namespace UseCases.Test.Store;

public class UpdateStoreImageUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store);

        var result = await useCase.Execute(new MemoryStream([1, 2, 3]), "store.jpg");

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Success_Without_Old_Image()
    {
        var store = StoreBuilder.Build();
        store.ImageUrl = string.Empty;
        var useCase = CreateUseCase(store);

        var result = await useCase.Execute(new MemoryStream([1, 2, 3]), "store.jpg");

        result.ShouldNotBeNull();
    }

    private static UpdateStoreImageUseCase CreateUseCase(IOrder.Domain.Entities.Store store)
    {
        var writeRepository = new StoreWriteOnlyRepositoryBuilder();
        writeRepository.GetByIdTracking(store);

        var readRepository = new StoreReadOnlyRepositoryBuilder();

        var storePermission = StorePermissionServiceBuilder.Build(store.Id);
        var unitOfWork = UnitOfWorkBuilder.Build();

        var storageService = new StorageServiceBuilder()
            .UploadImageAsync("http://new-store-image.jpg")
            .DeleteImageAsync()
            .Build();

        return new UpdateStoreImageUseCase(
            readRepository.Build(),
            writeRepository.Build(),
            storePermission,
            storageService,
            unitOfWork);
    }
}
