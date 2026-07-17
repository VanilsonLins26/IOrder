using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Product;

public class UpdateProductImageUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var store = StoreBuilder.Build(storeId: storeId);
        var product = ProductBuilder.Build(storeId);
        var useCase = CreateUseCase(store, product);

        var result = await useCase.Execute(product.Id, new MemoryStream([1, 2, 3]), "test.jpg");

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var product = ProductBuilder.Build();
        var useCase = CreateUseCase(store: null, product);

        Func<Task> act = async () => await useCase.Execute(product.Id, new MemoryStream([1, 2, 3]), "test.jpg");

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Loja não encontrada para este usuário.");
    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var store = StoreBuilder.Build();
        var useCase = CreateUseCase(store, product: null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid(), new MemoryStream([1, 2, 3]), "test.jpg");

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem()
            .ShouldBe("Produto não encontrado ou não pertence a sua loja.");
    }

    [Fact]
    public async Task Error_Product_Not_Belonging_To_Store()
    {
        var store = StoreBuilder.Build();
        var product = ProductBuilder.Build();
        var useCase = CreateUseCase(store, product);

        Func<Task> act = async () => await useCase.Execute(product.Id, new MemoryStream([1, 2, 3]), "test.jpg");

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem()
            .ShouldBe("Produto não encontrado ou não pertence a sua loja.");
    }

    private static UpdateProductImageUseCase CreateUseCase(
        IOrder.Domain.Entities.Store? store = null,
        IOrder.Domain.Entities.Product? product = null)
    {
        var userId = store?.UserId ?? "test-user-id";
        var loggedUser = LoggedUserBuilder.Build(userId);

        var storeReadRepository = new StoreReadOnlyRepositoryBuilder();
        if (store is not null)
            storeReadRepository.GetByUserIdAsync(store);

        var productReadRepository = new ProductReadOnlyRepositoryBuilder();
        if (product is not null)
            productReadRepository.GetByIdAsync(product);

        var productWriteRepository = new ProductWriteOnlyRepositoryBuilder()
            .Update()
            .Build();

        var unitOfWork = UnitOfWorkBuilder.Build();

        var storageService = new StorageServiceBuilder()
            .UploadImageAsync("http://new-image.jpg")
            .DeleteImageAsync()
            .Build();

        return new UpdateProductImageUseCase(
            productReadRepository.Build(),
            productWriteRepository,
            storeReadRepository.Build(),
            loggedUser,
            storageService,
            unitOfWork);
    }
}
