using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IMarkAsOutForDeliveryUseCase
{
    Task<OrderResponseDto> Execute(Guid orderId);
}

public class MarkAsOutForDeliveryUseCase : IMarkAsOutForDeliveryUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IDeliveryAssignmentReadOnlyRepository _deliveryReadOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsOutForDeliveryUseCase(
        ILoggedUserService loggedUserService,
        IStorePermissionService storePermissionService,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IDeliveryAssignmentReadOnlyRepository deliveryReadOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _deliveryReadOnlyRepository = deliveryReadOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponseDto> Execute(Guid orderId)
    {
        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        var userId = _loggedUserService.GetUserId();
        var isStoreOwner = order.StoreId.ToString() != null;

        try
        {
            await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);
        }
        catch
        {
            var assignment = await _deliveryReadOnlyRepository.GetByOrderIdAsync(orderId);
            if (assignment == null || assignment.CourierUserId != userId)
                throw new UnauthorizedStoreException([ResourceMessagesException.UNAUTHORIZED_STORE]);
        }

        if (order.Status != Domain.Entities.Enums.OrderStatus.Ready)
            throw new ErrorOnValidationException(["O pedido precisa estar como 'Pronto' para sair para entrega."]);

        order.MarkAsOutForDelivery();
        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        return order.Adapt<OrderResponseDto>();
    }
}
