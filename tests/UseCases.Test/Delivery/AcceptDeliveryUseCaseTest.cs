using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities;

using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Delivery;

public class AcceptDeliveryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        order.StartSearchingCourier();
        var useCase = CreateUseCase(order);

        var response = await useCase.Execute(order.Id);

        response.ShouldNotBeNull();
        response.OrderId.ShouldBe(order.Id);
        response.CourierUserId.ShouldBe("test-user-id");
    }

    [Fact]
    public async Task Error_OrderNotFound()
    {
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldContain("Pedido não encontrado.");
    }

    [Fact]
    public async Task Error_OrderNotSearchingCourier()
    {
        var order = OrderBuilder.Build();
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Este pedido não está buscando entregadores.");
    }

    [Fact]
    public async Task Error_OrderAlreadyAccepted()
    {
        var order = OrderBuilder.Build();
        order.StartSearchingCourier();
        var assignment = DeliveryAssignmentBuilder.Build();
        assignment.OrderId = order.Id;
        assignment.Accept();
        order.AssignCourier(assignment);
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Esta entrega já foi aceita por outro entregador.");
    }

    private AcceptDeliveryUseCase CreateUseCase(IOrder.Domain.Entities.Order? order, string userId = "test-user-id")
    {
        var loggedUser = LoggedUserBuilder.Build(userId);
        var orderWriteOnly = new OrderWriteOnlyRepositoryBuilder().GetByIdTracking(order).Build();
        var deliveryWriteOnly = new DeliveryAssignmentWriteOnlyRepositoryBuilder().Build();
        var uow = UnitOfWorkBuilder.Build();

        return new AcceptDeliveryUseCase(
            loggedUser,
            orderWriteOnly,
            deliveryWriteOnly,
            uow);
    }
}
