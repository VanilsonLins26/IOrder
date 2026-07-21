using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Commands;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Moq;

namespace UseCases.Test.Delivery;

public class BroadcastDeliveryOfferUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        order.GetType().GetProperty("IsSearchingCourier")!.SetValue(order, false);
        
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Order_Not_Found()
    {
        var useCase = CreateUseCase(null);

        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid());

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Error_Unauthorized_Store_Owner()
    {
        var order = OrderBuilder.Build();
        var useCase = CreateUseCase(order, authorized: false);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        await act.ShouldThrowAsync<UnauthorizedStoreException>();
    }

    [Fact]
    public async Task Error_Order_Status_Invalid()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Pending);
        
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("O pedido deve estar Pronto ou Sendo Preparado para solicitar entregador.");
    }

    [Fact]
    public async Task Error_Already_Searching_Courier()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        order.GetType().GetProperty("IsSearchingCourier")!.SetValue(order, true);
        
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("O pedido já está buscando entregador.");
    }

    [Fact]
    public async Task Error_Already_Has_Courier()
    {
        var order = OrderBuilder.Build();
        order.GetType().GetProperty("Status")!.SetValue(order, OrderStatus.Ready);
        order.GetType().GetProperty("IsSearchingCourier")!.SetValue(order, false);
        
        var assignment = DeliveryAssignmentBuilder.Build(null, AssignmentStatus.Accepted, order.Id);
        assignment.GetType().GetProperty("Status")!.SetValue(assignment, AssignmentStatus.Accepted);
        order.AssignCourier(assignment);
        
        var useCase = CreateUseCase(order);

        Func<Task> act = async () => await useCase.Execute(order.Id);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Esta entrega já possui um entregador.");
    }

    private IBroadcastDeliveryOfferUseCase CreateUseCase(
        IOrder.Domain.Entities.Order? order,
        bool authorized = true)
    {
        var orderWriteOnlyRepository = new OrderWriteOnlyRepositoryBuilder();
        if (order is not null)
            orderWriteOnlyRepository.GetByIdTracking(order);

        IOrder.Application.Services.StorePermission.IStorePermissionService storePermissionService;
        if (authorized && order is not null)
            storePermissionService = StorePermissionServiceBuilder.Build(order.StoreId);
        else if (authorized)
            storePermissionService = StorePermissionServiceBuilder.Build(Guid.Empty);
        else
        {
            var mock = new Moq.Mock<IOrder.Application.Services.StorePermission.IStorePermissionService>();
            mock.Setup(s => s.ValidateStoreOwnerAsync(It.IsAny<Guid>())).ThrowsAsync(new UnauthorizedStoreException([]));
            storePermissionService = mock.Object;
        }

        var unitOfWork = UnitOfWorkBuilder.Build();

        return new BroadcastDeliveryOfferUseCase(
            orderWriteOnlyRepository.Build(),
            storePermissionService,
            unitOfWork);
    }
}
