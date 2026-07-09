using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Product;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Communication.Request;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCases.Test.Product;

public class CreatePromotionPriceUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = RequestPromotionPriceBuilder.Build();

        var usecase = CreateUseCase(request.ProductId);

        var result = await usecase.Execute(request);

        result.ShouldNotBeNull();
        result.Price.ShouldBe(request.Price!.Value);
        result.ProductId.ShouldBe(request.ProductId);
        result.InitialTime.ShouldBe(request.InitialTime!.Value);
        result.FinalTime.ShouldBe(request.FinalTime!.Value);
    }

    [Fact]
    public async Task Error_Price_Empty()
    {
        var request = RequestPromotionPriceBuilder.Build();
        request.Price = null;

        var usecase = CreateUseCase(request.ProductId);

        Func<Task> act = async () => await usecase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PRICE_EMPTY);
    }

    [Fact]
    public async Task Error_Already_Exists_Promotion_In_Date()
    {
        var request = RequestPromotionPriceBuilder.Build();

        request.InitialTime = DateTime.UtcNow.AddDays(1);
        request.FinalTime = DateTime.UtcNow.AddDays(2);

        var usecase = CreateUseCase(request.ProductId, request.InitialTime, request.FinalTime);

        Func<Task> act = async () => await usecase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.EXISTS_PROMOTION_IN_THIS_DATE);
    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var request = RequestPromotionPriceBuilder.Build();

        var usecase = CreateUseCase(Guid.NewGuid());

        Func<Task> act = async () => await usecase.Execute(request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PRODUCT_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Promotion_Price_Invalid()
    {
        var request = RequestPromotionPriceBuilder.Build();
        request.Price = 60000m;

        var usecase = CreateUseCase(request.ProductId);

        Func<Task> act = async () => await usecase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PROMOTION_PRICE_INVALID);
    }

    private static CreatePromotionPriceUseCase CreateUseCase(Guid? productId = null, DateTime? initialTime = null, DateTime? finalTime = null)
    {
        var writeRepository = new ProductWriteOnlyRepositoryBuilder().CreatePromotion().Build();
        var readRepository = new ProductReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        if (productId != null)
            readRepository.GetByIdAsync(productId.Value);

        if (initialTime != null && finalTime != null)
            readRepository.ExistsPromotionInDate(productId!.Value, initialTime.Value, finalTime.Value);


        var storePermissionService = CommomTestUtilities.Services.StorePermissionServiceBuilder.Build();

        var eventDispatcher = new Mock<IDomainEventDispatcher>();

        return new CreatePromotionPriceUseCase(writeRepository, readRepository.Build(), unitOfWork, storePermissionService, new IOrder.Application.UseCases.Product.Commands.CreatePromotionPriceValidator(), eventDispatcher.Object);

    }
}
