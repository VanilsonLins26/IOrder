using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Category;
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

public class AddProductsToCategoryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);

        var product = ProductBuilder.Build(storeId);
        
        var request = AddProductsToCategoryRequestBuilder.Build(new List<Guid> { product.Id });
        var products = new List<IOrder.Domain.Entities.Product> { product };

        var useCase = CreateUseCase(storeId, category, products);

        var response = await useCase.Execute(category.Id, request);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(category.Name);
        
        // Assert that the product category ID was assigned
        product.CategoryId.ShouldBe(category.Id);
    }

    [Fact]
    public async Task Error_Empty_Products()
    {
        var storeId = Guid.NewGuid();
        var category = CategoryBuilder.Build(storeId);

        var request = AddProductsToCategoryRequestBuilder.Build(new List<Guid>());

        var useCase = CreateUseCase(storeId, category, new List<IOrder.Domain.Entities.Product>());

        Func<Task> act = async () => await useCase.Execute(category.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("A lista de produtos não pode estar vazia.");
    }

    private AddProductsToCategoryUseCase CreateUseCase(
        Guid storeId, 
        IOrder.Domain.Entities.Category? category = null, 
        List<IOrder.Domain.Entities.Product>? products = null)
    {
        var writeOnlyRepository = new CategoryWriteOnlyRepositoryBuilder();
        if (category is not null)
            writeOnlyRepository.GetByIdTracking(category.Id, category);

        products ??= new List<IOrder.Domain.Entities.Product>();
        var productWriteOnlyRepository = new ProductWriteOnlyRepositoryBuilder();
        
        var productIds = new List<Guid>();
        foreach(var p in products)
            productIds.Add(p.Id);

        productWriteOnlyRepository.GetByIdsTracking(productIds, products);

        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);

        return new AddProductsToCategoryUseCase(
            writeOnlyRepository.Build(),
            productWriteOnlyRepository.Build(),
            permissionService,
            uow);
    }
}
