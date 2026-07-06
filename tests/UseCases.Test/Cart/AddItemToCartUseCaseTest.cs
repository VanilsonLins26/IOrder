using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using IOrder.Communication.Request;

namespace UseCases.Test.Cart;

public class AddItemToCartUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var cart = CartBuilder.Build();
        var request = AddItemToCartRequestBuilder.Build();
        var useCase = CreateUseCase(cart, true, request);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
    }

    [Fact]
    public async Task Error_Product_Not_Found()
    {
        var cart = CartBuilder.Build();
        var request = AddItemToCartRequestBuilder.Build();
        var useCase = CreateUseCase(cart, false, request); // Product does not exist

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.PRODUCT_NOT_FOUND);
    }

    private AddItemToCartUseCase CreateUseCase(IOrder.Domain.Entities.Cart cart, bool productExists, AddItemToCartRequestDto request)
    {
        var readOnlyRepository = new CartReadOnlyRepositoryBuilder().GetCartAsync(cart).Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        var loggedUserService = LoggedUserBuilder.Build(cart.UserId);

        var productReadOnlyBuilder = new ProductReadOnlyRepositoryBuilder();
        if (productExists)
        {
            var product = new IOrder.Domain.Entities.Product { Id = request.ProductId };
            productReadOnlyBuilder.GetByIdAsync(product);
        }
        else
        {
            productReadOnlyBuilder.GetByIdReturnsNull(request.ProductId);
        }

        var validator = new AddItemToCartValidator();

        return new AddItemToCartUseCase(
            loggedUserService,
            readOnlyRepository,
            writeOnlyRepository,
            validator,
            productReadOnlyBuilder.Build());
    }
}
