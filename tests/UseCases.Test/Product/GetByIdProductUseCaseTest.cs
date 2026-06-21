using CommomTestUtilities.Repositories;
using IOrder.Application.UseCases.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCases.Test.Product;

public class GetByIdProductUseCaseTest
{
    [Fact]
    public async Task Succes()
    {
        var requestId = Guid.CreateVersion7();

        var useCase = CreateUseCase(requestId);

        var result = await useCase.Execute(requestId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(requestId);

    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var requestId = Guid.NewGuid();

        var useCase = CreateUseCase(requestId);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PRODUCT_NOT_FOUND);

    }

    private static GetProductById CreateUseCase(Guid productId)
    {
        var readRepository = new ProductReadOnlyRepositoryBuilder();

        readRepository.GetByIdAsync(productId);



        return new GetProductById(readRepository.Build());

    }
}

