using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Domain.Entities;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Cart;

public class ChangeQuantityUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var cart = CartBuilder.Build();
        var cartItem = CartItemBuilder.Build();
        cart.Items.Add(cartItem);

        var request = ChangeCartItemQuantityRequestBuilder.Build();
        request.CartItemId = cartItem.Id;

        var useCase = CreateUseCase(cart);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
    }

    [Fact]
    public async Task Error_Item_Not_Found()
    {
        var cart = CartBuilder.Build();
        var request = ChangeCartItemQuantityRequestBuilder.Build();

        var useCase = CreateUseCase(cart);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe(ResourceMessagesException.CART_ITEM_NOT_FOUND);
    }

    private ChangeQuantityUseCase CreateUseCase(IOrder.Domain.Entities.Cart cart)
    {
        var readOnlyRepository = new CartReadOnlyRepositoryBuilder().GetCartAsync(cart).Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        var loggedUserService = LoggedUserBuilder.Build(cart.UserId);

        return new ChangeQuantityUseCase(
            readOnlyRepository,
            writeOnlyRepository,
            loggedUserService);
    }
}
