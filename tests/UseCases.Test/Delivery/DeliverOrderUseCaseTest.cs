using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Delivery;

public class DeliverOrderUseCaseTest
{
    [Fact]
    public async Task Success_From_PickedUp()
    {
        var loggedUser = LoggedUserBuilder.Build("test-courier-id");
        var assignment = DeliveryAssignmentBuilder.Build(courierUserId: "test-courier-id");
        assignment.GetType().GetProperty("Status")!.SetValue(assignment, AssignmentStatus.PickedUp);
        
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        
        var useCase = CreateUseCase(assignment, order, "test-courier-id");

        var result = await useCase.Execute(assignment.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(assignment.Id);
    }

    [Fact]
    public async Task Success_From_InTransit()
    {
        var loggedUser = LoggedUserBuilder.Build("test-courier-id");
        var assignment = DeliveryAssignmentBuilder.Build(courierUserId: "test-courier-id");
        assignment.GetType().GetProperty("Status")!.SetValue(assignment, AssignmentStatus.InTransit);
        
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.OutForDelivery);
        
        var useCase = CreateUseCase(assignment, order, "test-courier-id");

        var result = await useCase.Execute(assignment.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(assignment.Id);
    }

    [Fact]
    public async Task Error_Assignment_Not_Found()
    {
        var useCase = CreateUseCase(null, null, "test-courier-id");

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Error_Unauthorized()
    {
        var assignment = DeliveryAssignmentBuilder.Build(courierUserId: "another-courier-id");
        var useCase = CreateUseCase(assignment, null, "test-courier-id");

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        await act.ShouldThrowAsync<UnauthorizedStoreException>();
    }

    [Fact]
    public async Task Error_Status_Invalid()
    {
        var assignment = DeliveryAssignmentBuilder.Build(courierUserId: "test-courier-id");
        assignment.GetType().GetProperty("Status")!.SetValue(assignment, AssignmentStatus.Pending);
        
        var useCase = CreateUseCase(assignment, null, "test-courier-id");

        Func<Task> act = async () => await useCase.Execute(assignment.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Este pedido não está em trânsito.");
    }

    private IDeliverOrderUseCase CreateUseCase(
        IOrder.Domain.Entities.DeliveryAssignment? assignment,
        IOrder.Domain.Entities.Order? order,
        string loggedUserId)
    {
        var loggedUserService = LoggedUserBuilder.Build(loggedUserId);
        
        var writeOnlyRepository = new DeliveryAssignmentWriteOnlyRepositoryBuilder();
        if (assignment is not null)
            writeOnlyRepository.GetByIdTrackingAsync(assignment);

        var orderWriteOnlyRepository = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnlyRepository.GetByIdTracking(order);

        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeliverOrderUseCase(
            loggedUserService,
            writeOnlyRepository.Build(),
            orderWriteOnlyRepository.Build(),
            unitOfWork);
    }
}
