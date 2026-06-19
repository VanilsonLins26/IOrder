using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests;
using IOrder.Application.UseCases.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCases.Test.Product;

public class DeleteProductUseCaseTest
{
    [Fact]
    public async Task Succes()
    {
        var requestId = Guid.CreateVersion7();

        var useCase = CreateUseCase(requestId);

        var result = await useCase.Execute(requestId);

        result.ShouldNotBeNull();
        result.Price.ShouldBe(50000m);
    
    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var requestId = Guid.NewGuid();

        var useCase = CreateUseCase(requestId);

        Func<Task> act = async() =>  await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PRODUCT_NOT_FOUND);

    }

    private static DeleteProductUseCase CreateUseCase(Guid productId)
    {
        var writeRepository = new ProductWriteOnlyRepositoryBuilder().Create().Build();
        var readRepository = new ProductReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        readRepository.GetByIdAsync(productId);



        return new DeleteProductUseCase(writeRepository, readRepository.Build(), unitOfWork);

    }
}
