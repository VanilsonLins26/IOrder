using FluentValidation;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;
using IOrder.Application.Services.StorePermission;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Order.Commands;

public class UpdateOrderStatusUseCase : IUpdateOrderStatusUseCase
{
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IValidator<Communication.Request.UpdateOrderStatusRequestDto> _validator;

    public UpdateOrderStatusUseCase(
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IStorePermissionService storePermissionService,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        IValidator<Communication.Request.UpdateOrderStatusRequestDto> validator)
    {
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _storePermissionService = storePermissionService;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
        _validator = validator;
    }

    public async Task<OrderResponseDto> Execute(Guid id, Communication.Request.UpdateOrderStatusRequestDto request)
    {
        await Validate(request);

        var order = await _orderWriteOnlyRepository.GetByIdTracking(id)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);

        var newStatus = request.Status switch
        {
            Communication.Enums.OrderStatusDto.AwaitingPayment => Domain.Entities.Enums.OrderStatus.AwaitingPayment,
            Communication.Enums.OrderStatusDto.Declined => Domain.Entities.Enums.OrderStatus.Declined,
            Communication.Enums.OrderStatusDto.Preparing => Domain.Entities.Enums.OrderStatus.Preparing,
            Communication.Enums.OrderStatusDto.Ready => Domain.Entities.Enums.OrderStatus.Ready,
            Communication.Enums.OrderStatusDto.Delivered => Domain.Entities.Enums.OrderStatus.Delivered,
            Communication.Enums.OrderStatusDto.Cancelled => Domain.Entities.Enums.OrderStatus.Cancelled,
            _ => throw new ErrorOnValidationException([ResourceMessagesException.ORDER_INVALID_STATUS])
        };

        switch (newStatus)
        {
            case Domain.Entities.Enums.OrderStatus.AwaitingPayment:
                order.Accept();
                break;
            case Domain.Entities.Enums.OrderStatus.Declined:
                order.Decline();
                break;
            case Domain.Entities.Enums.OrderStatus.Preparing:
                order.MarkAsPreparing();
                break;
            case Domain.Entities.Enums.OrderStatus.Ready:
                order.MarkAsReady();
                break;
            case Domain.Entities.Enums.OrderStatus.Delivered:
                order.MarkAsDelivered();
                break;
            case Domain.Entities.Enums.OrderStatus.Cancelled:
                order.Cancel();
                break;
            default:
                throw new ErrorOnValidationException([ResourceMessagesException.ORDER_INVALID_STATUS]);
        }

        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        var events = order.DomainEvents.ToList();
        order.ClearDomainEvents();
        await _domainEventDispatcher.DispatchAsync(events);

        var response = await _orderWriteOnlyRepository.GetByIdTracking(id);
        return response.Adapt<OrderResponseDto>();
    }

    private async Task Validate(Communication.Request.UpdateOrderStatusRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
