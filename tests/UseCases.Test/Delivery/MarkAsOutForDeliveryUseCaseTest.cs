using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Moq;
using Shouldly;

namespace UseCases.Test.Delivery;

public class MarkAsOutForDeliveryUseCaseTest
{
    [Fact]
    public async Task Success_As_Store_Owner()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        
        var useCase = CreateUseCase(order, authorizedAsStoreOwner: true, "store-owner-id");

        var result = await useCase.Execute(order.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
    }

    [Fact]
    public async Task Success_As_Courier()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        
        var assignment = DeliveryAssignmentBuilder.Build("test-courier-id", AssignmentStatus.Accepted, order.Id);
        
        var useCase = CreateUseCase(order, authorizedAsStoreOwner: false, "test-courier-id", assignment);

        var result = await useCase.Execute(order.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var useCase = CreateUseCase(null, authorizedAsStoreOwner: true, "store-owner-id");

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Error_Unauthorized()
    {
        var order = OrderBuilder.Build();
        var assignment = CommomTestUtilities.Entities.DeliveryAssignmentBuilder.Build("different-courier-id");
        assignment.OrderId = order.Id;
        
        var useCase = CreateUseCase(order, authorizedAsStoreOwner: false, "test-user-id", assignment);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        await act.ShouldThrowAsync<UnauthorizedStoreException>();
    }

    [Fact]
    public async Task Error_Status_Invalid()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Pending);
        
        var useCase = CreateUseCase(order, authorizedAsStoreOwner: true, "store-owner-id");

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("O pedido precisa estar como 'Pronto' para sair para entrega.");
    }

    private IMarkAsOutForDeliveryUseCase CreateUseCase(
        IOrder.Domain.Entities.Order? order,
        bool authorizedAsStoreOwner,
        string loggedUserId,
        IOrder.Domain.Entities.DeliveryAssignment? assignment = null)
    {
        var loggedUserService = LoggedUserBuilder.Build(loggedUserId);
        
        IOrder.Application.Services.StorePermission.IStorePermissionService storePermissionService;
        if (authorizedAsStoreOwner)
            storePermissionService = StorePermissionServiceBuilder.Build(order?.StoreId);
        else
        {
            var mock = new Moq.Mock<IOrder.Application.Services.StorePermission.IStorePermissionService>();
            mock.Setup(s => s.ValidateStoreOwnerAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedStoreException([]));
            storePermissionService = mock.Object;
        }

        var orderWriteOnlyRepository = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnlyRepository.GetByIdTracking(order);

        var deliveryReadOnlyRepository = new DeliveryAssignmentReadOnlyRepositoryBuilder();
        if (assignment is not null)
            deliveryReadOnlyRepository.GetByOrderIdAsync(assignment);

        var unitOfWork = UnitOfWorkBuilder.Build();

        return new MarkAsOutForDeliveryUseCase(
            loggedUserService,
            storePermissionService,
            orderWriteOnlyRepository.Build(),
            deliveryReadOnlyRepository.Build(),
            unitOfWork);
    }
}
