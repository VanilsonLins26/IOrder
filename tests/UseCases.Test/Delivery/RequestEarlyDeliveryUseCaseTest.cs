using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System.Reflection;

namespace UseCases.Test.Delivery;

public class RequestEarlyDeliveryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        typeof(IOrder.Domain.Entities.Order).GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        typeof(IOrder.Domain.Entities.Order).GetProperty("RequestedEarlyDelivery")!.SetValue(order, false);
        typeof(IOrder.Domain.Entities.Order).GetProperty("DeliveryType")!.SetValue(order, DeliveryType.Delivery);

        var useCase = CreateUseCase(order.UserId, order);

        var response = await useCase.Execute(order.Id);

        response.ShouldNotBeNull();
        response.RequestedEarlyDelivery.ShouldBeTrue();
    }

    [Fact]
    public async Task Error_OrderNotFound()
    {
        var useCase = CreateUseCase("user-1", null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.ORDER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_UnauthorizedStore()
    {
        var order = OrderBuilder.Build();
        order.UserId = "user-1";
        
        var useCase = CreateUseCase("user-2", order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<UnauthorizedStoreException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.UNAUTHORIZED_STORE);
    }

    [Fact]
    public async Task Error_StatusNotReady()
    {
        var order = OrderBuilder.Build();
        typeof(IOrder.Domain.Entities.Order).GetProperty("Status")!.SetValue(order, OrderStatus.Preparing);
        
        var useCase = CreateUseCase(order.UserId, order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("O pedido precisa estar como 'Pronto' para solicitar entrega antecipada.");
    }

    [Fact]
    public async Task Error_AlreadyRequested()
    {
        var order = OrderBuilder.Build();
        typeof(IOrder.Domain.Entities.Order).GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        typeof(IOrder.Domain.Entities.Order).GetProperty("RequestedEarlyDelivery")!.SetValue(order, true);
        
        var useCase = CreateUseCase(order.UserId, order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Este pedido já possui uma solicitação de entrega antecipada.");
    }

    [Fact]
    public async Task Error_NotDeliveryType()
    {
        var order = OrderBuilder.Build();
        typeof(IOrder.Domain.Entities.Order).GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        typeof(IOrder.Domain.Entities.Order).GetProperty("RequestedEarlyDelivery")!.SetValue(order, false);
        typeof(IOrder.Domain.Entities.Order).GetProperty("DeliveryType")!.SetValue(order, DeliveryType.Pickup);
        
        var useCase = CreateUseCase(order.UserId, order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Solicitação de entrega antecipada disponível apenas para entregas.");
    }

    private RequestEarlyDeliveryUseCase CreateUseCase(
        string loggedUserId,
        IOrder.Domain.Entities.Order? order)
    {
        var loggedUser = LoggedUserBuilder.Build(loggedUserId);
        
        var orderWriteOnly = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnly.GetByIdTracking(order);

        var uow = UnitOfWorkBuilder.Build();

        return new RequestEarlyDeliveryUseCase(
            loggedUser,
            orderWriteOnly.Build(),
            uow);
    }
}
