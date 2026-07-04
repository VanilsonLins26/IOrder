using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Product;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCases.Test.Product;

public class UpdateProductUseCaseTest
{
    [Fact]
    public async Task Succes()
    {
        var request = RequestUpdateProductBuilder.Build();
        var requestId = Guid.CreateVersion7();

        var useCase = CreateUseCase(requestId);

        var result = await useCase.Execute(requestId, request);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Description.ShouldBe(request.Description);
        result.ImageUrl.ShouldBe(request.ImageUrl);
        result.Price.ShouldBe(request.Price!.Value);
        result.Id.ShouldBe(requestId);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var request = RequestUpdateProductBuilder.Build();
        var requestId = Guid.CreateVersion7();

        var useCase = CreateUseCase(requestId, request.Name);

        Func<Task> act = async () => await useCase.Execute(requestId, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_ALREADY_EXISTS);

    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestUpdateProductBuilder.Build();
        request.Name = string.Empty;
        var requestId = Guid.CreateVersion7();

        var useCase = CreateUseCase(requestId);

        Func<Task> act = async () => await useCase.Execute(requestId, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    private static UpdateProductUseCase CreateUseCase(Guid productId, string? productName = null)
    {
        var writeRepository = new ProductWriteOnlyRepositoryBuilder();
        var readRepository = new ProductReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        writeRepository.GetByIdTracking(productId);

        if (string.IsNullOrEmpty(productName) == false)
            readRepository.NameExists(productName);



        var storePermissionService = CommomTestUtilities.Services.StorePermissionServiceBuilder.Build();

        return new UpdateProductUseCase(writeRepository.Build(), unitOfWork, readRepository.Build(), storePermissionService);

    }
}
