using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Category;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Category;

public class EmptyCategoryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);

        var product = ProductBuilder.Build(storeId);
        product.CategoryId = category.Id;
        
        var products = new List<IOrder.Domain.Entities.Product> { product };

        var useCase = CreateUseCase(storeId, category, products);

        var response = await useCase.Execute(category.Id);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(category.Name);
        
        // Assert that the product category ID was removed
        product.CategoryId.ShouldBeNull();
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

    private EmptyCategoryUseCase CreateUseCase(
        Guid storeId, 
        IOrder.Domain.Entities.Category? category = null, 
        List<IOrder.Domain.Entities.Product>? categoryProducts = null)
    {
        var writeOnlyRepository = new CategoryWriteOnlyRepositoryBuilder();
        if (category is not null)
            writeOnlyRepository.GetByIdTracking(category.Id, category);

        categoryProducts ??= new List<IOrder.Domain.Entities.Product>();
        
        var productWriteOnlyRepository = new ProductWriteOnlyRepositoryBuilder();
        productWriteOnlyRepository.GetByIdsTracking(new List<Guid> { categoryProducts.Count > 0 ? categoryProducts[0].Id : Guid.NewGuid() }, categoryProducts);
        
        var productReadOnlyRepository = new ProductReadOnlyRepositoryBuilder();
        productReadOnlyRepository.GetAll(categoryProducts);

        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new EmptyCategoryUseCase(
            writeOnlyRepository.Build(),
            productWriteOnlyRepository.Build(),
            productReadOnlyRepository.Build(),
            permissionService,
            uow);
    }
}
