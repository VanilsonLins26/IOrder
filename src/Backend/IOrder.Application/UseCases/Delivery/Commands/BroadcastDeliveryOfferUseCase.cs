using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Application.Services.StorePermission;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IBroadcastDeliveryOfferUseCase
{
    Task Execute(Guid orderId);
}

public class BroadcastDeliveryOfferUseCase : IBroadcastDeliveryOfferUseCase
{
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IUnitOfWork _unitOfWork;

    public BroadcastDeliveryOfferUseCase(
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IStorePermissionService storePermissionService,
        IUnitOfWork unitOfWork)
    {
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _storePermissionService = storePermissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid orderId)
    {
        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException(["Pedido não encontrado."]);

        await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);

        if (order.Status != Domain.Entities.Enums.OrderStatus.Ready && order.Status != Domain.Entities.Enums.OrderStatus.Preparing)
            throw new ErrorOnValidationException(["O pedido deve estar Pronto ou Sendo Preparado para solicitar entregador."]);
            
        if (order.IsSearchingCourier)
            throw new ErrorOnValidationException(["O pedido já está buscando entregador."]);

        if (order.Assignments.Any(a => 
            a.Status != Domain.Entities.Enums.AssignmentStatus.Rejected 
            && a.Status != Domain.Entities.Enums.AssignmentStatus.Failed))
        {
            throw new ErrorOnValidationException(["Esta entrega já possui um entregador."]);
        }

        order.StartSearchingCourier();

        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();
    }
}
