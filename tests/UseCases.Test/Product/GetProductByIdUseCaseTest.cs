using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Product;

public class GetProductByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var product = ProductBuilder.Build();
        var repositoryBuilder = new ProductReadOnlyRepositoryBuilder();
        repositoryBuilder.GetByIdAsync(product);

        var useCase = new GetProductByIdUseCase(repositoryBuilder.Build());

        var response = await useCase.Execute(product.Id);

        response.ShouldNotBeNull();
        response.Name.ShouldBe(product.Name);
    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var repositoryBuilder = new ProductReadOnlyRepositoryBuilder();
        repositoryBuilder.GetByIdReturnsNull(Guid.NewGuid());

        var useCase = new GetProductByIdUseCase(repositoryBuilder.Build());

        var exception = await Should.ThrowAsync<NotFoundException>(() => useCase.Execute(Guid.NewGuid()));
        exception.GetErrorMessages().ShouldContain(ResourceMessagesException.PRODUCT_NOT_FOUND);
    }
}
