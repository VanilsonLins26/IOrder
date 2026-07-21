using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System.Reflection;

namespace UseCases.Test.Delivery;

public class PickupOrderUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        assignment.Accept();
        
        var order = OrderBuilder.Build();
        typeof(IOrder.Domain.Entities.Order).GetProperty("Id")!.SetValue(order, assignment.OrderId);
        typeof(IOrder.Domain.Entities.Order).GetProperty("Status")!.SetValue(order, OrderStatus.Ready);

        var useCase = CreateUseCase(assignment.CourierUserId, assignment, order);

        var response = await useCase.Execute(assignment.Id);

        response.ShouldNotBeNull();
        response.Status.ShouldBe(IOrder.Communication.Enums.AssignmentStatusDto.PickedUp);
    }

    [Fact]
    public async Task Error_AssignmentNotFound()
    {
        var useCase = CreateUseCase("courier-1", null, null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Atribuição não encontrada.");
    }

    [Fact]
    public async Task Error_CourierNotMatch()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        assignment.CourierUserId = "courier-1";
        
        var useCase = CreateUseCase("courier-2", assignment, null);

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        var exception = await act.ShouldThrowAsync<UnauthorizedStoreException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Você não tem permissão para buscar este pedido.");
    }

    [Fact]
    public async Task Error_StatusNotAccepted()
    {
        var assignment = DeliveryAssignmentBuilder.Build();
        // assignment is initially Pending
        
        var useCase = CreateUseCase(assignment.CourierUserId, assignment, null);

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Este pedido ainda não foi aceito pelo entregador.");
    }

    private PickupOrderUseCase CreateUseCase(
        string loggedUserId,
        IOrder.Domain.Entities.DeliveryAssignment? assignment,
        IOrder.Domain.Entities.Order? order)
    {
        var loggedUser = LoggedUserBuilder.Build(loggedUserId);
        
        var assignmentWriteOnly = new DeliveryAssignmentWriteOnlyRepositoryBuilder();
        if (assignment is not null)
            assignmentWriteOnly.GetByIdTrackingAsync(assignment);

        var orderWriteOnly = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnly.GetByIdTracking(order);

        var uow = UnitOfWorkBuilder.Build();

        return new PickupOrderUseCase(
            loggedUser,
            assignmentWriteOnly.Build(),
            orderWriteOnly.Build(),
            uow);
    }
}
