using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Cart.Queries;
using IOrder.Domain.Entities;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UseCases.Test.Cart;

public class GetCartUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var cart = CartBuilder.Build();
        var product = ProductBuilder.Build();
        var cartItem = CartItemBuilder.Build(product.Id);
        cart.Items.Add(cartItem);

        var useCase = CreateUseCase(cart, product);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Items.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Success_Empty_Cart()
    {
        var cart = CartBuilder.Build(); // Empty by default
        var useCase = CreateUseCase(cart);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.Items.ShouldBeEmpty();
    }

    private GetCartUseCase CreateUseCase(IOrder.Domain.Entities.Cart cart, IOrder.Domain.Entities.Product? product = null)
    {
        var readOnlyRepository = new CartReadOnlyRepositoryBuilder().GetCartAsync(cart).Build();
        var writeOnlyRepository = new CartWriteOnlyRepositoryBuilder().Build();
        var loggedUserService = LoggedUserBuilder.Build(cart.UserId);

        var productReadOnlyBuilder = new ProductReadOnlyRepositoryBuilder();
        if (product != null)
        {
            var prices = new System.Collections.Generic.Dictionary<Guid, decimal>
            {
                { product.Id, 10m }
            };
            productReadOnlyBuilder.GetProductPricesByIds(prices);
        }

        var couponReadOnlyRepository = new CouponReadOnlyRepositoryBuilder()
            .GetByCodeAsync(null)
            .Build();

        return new GetCartUseCase(
            loggedUserService,
            readOnlyRepository,
            productReadOnlyBuilder.Build(),
            writeOnlyRepository,
            couponReadOnlyRepository);
    }
}
