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

public class CreateProductUseCaseTest
{

    [Fact]
    public async Task Succes()
    {
        var request = RequestCreateProductBuilder.Build();

        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Description.ShouldBe(request.Description);
        result.ImageUrl.ShouldBe(request.ImageUrl);
        result.Customizable.ShouldBe(request.Customizable);
        result.Price.ShouldBe(request.Price!.Value);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var request = RequestCreateProductBuilder.Build();

        var useCase = CreateUseCase(request.Name);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_ALREADY_EXISTS);
            
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestCreateProductBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.NAME_EMPTY);
    }

    private static CreateProductUseCase CreateUseCase(string? productName = null)
    {
        var writeRepository = new ProductWriteOnlyRepositoryBuilder().Create().Build();
        var readRepository = new ProductReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        if (string.IsNullOrEmpty(productName) == false)
            readRepository.NameExists(productName);

        

        return new CreateProductUseCase(writeRepository, unitOfWork, readRepository.Build());

    }
}
