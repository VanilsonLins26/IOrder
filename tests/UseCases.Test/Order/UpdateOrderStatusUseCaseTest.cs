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

public class UpdateOrderStatusUseCaseTest
{
    [Fact]
    public async Task Success_Accept()
    {
        var order = OrderBuilder.Build();
        var request = UpdateOrderStatusRequestBuilder.Build();
        request.Status = OrderStatusDto.AwaitingPayment;
        var useCase = CreateUseCase(order);

        var response = await useCase.Execute(order.Id, request);

        response.ShouldNotBeNull();
        response.Status.ShouldBe(OrderStatusDto.AwaitingPayment);
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var request = UpdateOrderStatusRequestBuilder.Build();
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Invalid_Status()
    {
        var order = OrderBuilder.Build();
        var request = UpdateOrderStatusRequestBuilder.Build();
        request.Status = (OrderStatusDto)999;
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
    }

    private static UpdateOrderStatusUseCase CreateUseCase(IOrder.Domain.Entities.Order? order)
    {
        var writeOnly = new OrderWriteOnlyRepositoryBuilder()
            .GetByIdTracking(order)
            .Build();
        var uow = UnitOfWorkBuilder.Build();
        var permissionService = StorePermissionServiceBuilder.Build(order?.StoreId ?? Guid.NewGuid());
        var validator = new UpdateOrderStatusValidator();
        var loggedUser = LoggedUserBuilder.Build(Guid.NewGuid()); // Store owner logic doesn't require user matching

        var eventDispatcher = new Mock<IDomainEventDispatcher>();

        return new UpdateOrderStatusUseCase(writeOnly, permissionService, uow, eventDispatcher.Object, validator, loggedUser);
    }
}
