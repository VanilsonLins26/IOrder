using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
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

public class RemoveItemUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var cart = CartBuilder.Build();
        var cartItem = CartItemBuilder.Build();
        cart.Items.Add(cartItem);

        var useCase = CreateUseCase(cart);

        var response = await useCase.Execute(cartItem.Id);

        response.ShouldNotBeNull();
        response.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task Error_Item_Not_Found()
    {
        var cart = CartBuilder.Build();
        var useCase = CreateUseCase(cart);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe(ResourceMessagesException.CART_ITEM_NOT_FOUND);
    }

    private RemoveItemUseCase CreateUseCase(IOrder.Domain.Entities.Cart cart)
    {
        var readOnlyRepository = new CartReadOnlyRepositoryBuilder().GetCartAsync(cart).Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        var loggedUserService = LoggedUserBuilder.Build(cart.UserId);

        return new RemoveItemUseCase(
            readOnlyRepository,
            writeOnlyRepository,
            loggedUserService);
    }
}
