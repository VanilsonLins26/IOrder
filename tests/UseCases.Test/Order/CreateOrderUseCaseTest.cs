using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Requests.Order;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Profile;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Moq;
using Shouldly;

namespace UseCases.Test.Order;

public class CreateOrderUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = CreateOrderRequestBuilder.Build();
        var cart = CartBuilder.Build();
        cart.CouponCode = string.Empty;
        cart.Items.AddRange(CartItemBuilder.BuildCollection(2));
        var useCase = CreateUseCase(cart);

        var response = await useCase.Execute(request);

        response.ShouldNotBeNull();
        response.CustomerNotes.ShouldBe(request.CustomerNotes);
        response.TotalAmount.ShouldBe(cart.CartTotal);
    }

    [Fact]
    public async Task Error_Empty_Cart()
    {
        var request = CreateOrderRequestBuilder.Build();
        var cart = CartBuilder.Build();
        var useCase = CreateUseCase(cart);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_EMPTY_CART);
    }

    [Fact]
    public async Task Error_Invalid_Cart()
    {
        var request = CreateOrderRequestBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.INVALID_CART);
    }

    [Fact]
    public async Task Error_Validation_Failed()
    {
        var request = CreateOrderRequestBuilder.Build();
        request.DeliveryDate = DateTime.UtcNow.AddDays(-1);
        var cart = CartBuilder.Build();
        cart.Items.AddRange(CartItemBuilder.BuildCollection(2));
        var useCase = CreateUseCase(cart);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_DELIVERY_DATE_IN_PAST);
    }

    private CreateOrderUseCase CreateUseCase(IOrder.Domain.Entities.Cart? cart, string userId = "test-user-id")
    {
        var cartReadOnly = new CartReadOnlyRepositoryBuilder();
        if (cart is not null)
            cartReadOnly.GetCartAsync(cart);

        var cartWriteOnly = new CartWriteOnlyRepositoryBuilder();
        var loggedUser = LoggedUserBuilder.Build(userId);
        var orderWriteOnly = new OrderWriteOnlyRepositoryBuilder().Create();
        var productReadOnly = new ProductReadOnlyRepositoryBuilder();
        var couponReadOnly = new CouponReadOnlyRepositoryBuilder();
        var uow = UnitOfWorkBuilder.Build();
        var validator = new CreateOrderValidator();

        var eventDispatcher = new Mock<IDomainEventDispatcher>();
        var profileReadOnly = new ProfileReadOnlyRepositoryBuilder().Build();
        var profileWriteOnly = new Mock<IProfileWriteOnlyRepository>();

        return new CreateOrderUseCase(
            loggedUser,
            cartReadOnly.Build(),
            cartWriteOnly.Build(),
            orderWriteOnly.Build(),
            productReadOnly.Build(),
            couponReadOnly.Build(),
            profileReadOnly,
            profileWriteOnly.Object,
            uow,
            eventDispatcher.Object,
            validator);
    }
}
