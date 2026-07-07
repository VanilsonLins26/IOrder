using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Order;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Order.Commands;
using IOrder.Communication.Enums;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Moq;
using Shouldly;

namespace UseCases.Test.Order;

public class NegotiateOrderUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        var request = NegotiateOrderRequestBuilder.Build();
        var useCase = CreateUseCase(order);

        var response = await useCase.Execute(order.Id, request);

        response.ShouldNotBeNull();
        response.Status.ShouldBe(OrderStatusDto.Negotiating);
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = NegotiateOrderRequestBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Validation_Failed()
    {
        var order = OrderBuilder.Build();
        var request = NegotiateOrderRequestBuilder.Build();
        request.ShopkeeperNotes = string.Empty;
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_MESSAGE_EMPTY);
    }

    private static NegotiateOrderUseCase CreateUseCase(IOrder.Domain.Entities.Order? order)
    {
        var storeId = order?.StoreId ?? Guid.NewGuid();
        var writeOnly = new OrderWriteOnlyRepositoryBuilder()
            .GetByIdTracking(order)
            .Build();
        var loggedUser = LoggedUserBuilder.Build(order?.UserId ?? "test-user-id");
        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(storeId);
        var validator = new NegotiateOrderValidator();

        var publisher = new Mock<IOrderMessagePublisher>();

        return new NegotiateOrderUseCase(writeOnly, permissionService, loggedUser, uow, publisher.Object, validator);
    }
}
