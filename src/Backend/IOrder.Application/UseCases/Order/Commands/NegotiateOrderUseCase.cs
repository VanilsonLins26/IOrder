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

public interface INegotiateOrderUseCase
{
    Task<OrderResponseDto> Execute(Guid id, Communication.Request.NegotiateOrderRequestDto request);
}

public class NegotiateOrderUseCase : INegotiateOrderUseCase
{
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IStorePermissionService _storePermissionService;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderMessagePublisher _messagePublisher;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IValidator<Communication.Request.NegotiateOrderRequestDto> _validator;

    public NegotiateOrderUseCase(
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IStorePermissionService storePermissionService,
        ILoggedUserService loggedUserService,
        IUnitOfWork unitOfWork,
        IOrderMessagePublisher messagePublisher,
        IDomainEventDispatcher domainEventDispatcher,
        IValidator<Communication.Request.NegotiateOrderRequestDto> validator)
    {
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _storePermissionService = storePermissionService;
        _loggedUserService = loggedUserService;
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
        _domainEventDispatcher = domainEventDispatcher;
        _validator = validator;
    }

    public async Task<OrderResponseDto> Execute(Guid id, Communication.Request.NegotiateOrderRequestDto request)
    {
        await Validate(request);

        var order = await _orderWriteOnlyRepository.GetByIdTracking(id)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);

        var message = new Domain.Entities.OrderMessage
        {
            UserId = _loggedUserService.GetUserId(),
            UserRole = "ShopKeeper",
            Message = request.ShopkeeperNotes ?? "Proposal sent",
            Type = Domain.Entities.Enums.MessageType.Proposal,
            ProposedTotalAmount = request.ProposedTotalAmount,
            ProposedDeliveryDate = request.ProposedDeliveryDate
        };

        message.OrderId = order.Id;
        order.AddMessage(message);
        _orderWriteOnlyRepository.AddOrderMessage(message);
        order.Negotiate(request.ProposedTotalAmount, request.ProposedDeliveryDate, request.ShopkeeperNotes);

        await _unitOfWork.Commit();

        var events = order.DomainEvents.ToList();
        order.ClearDomainEvents();
        await _domainEventDispatcher.DispatchAsync(events);

        await _messagePublisher.PublishMessageAsync(id, order.Adapt<OrderResponseDto>());
        return order.Adapt<OrderResponseDto>();
    }

    private async Task Validate(Communication.Request.NegotiateOrderRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
