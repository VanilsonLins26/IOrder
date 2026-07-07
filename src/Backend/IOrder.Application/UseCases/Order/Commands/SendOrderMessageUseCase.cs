using FluentValidation;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Order.Commands;

public interface ISendOrderMessageUseCase
{
    Task<OrderResponseDto> Execute(Guid orderId, Communication.Request.SendOrderMessageRequestDto request);
}

public class SendOrderMessageUseCase : ISendOrderMessageUseCase
{
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IStoreReadOnlyRepository _storeReadOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Communication.Request.SendOrderMessageRequestDto> _validator;

    public SendOrderMessageUseCase(
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IStoreReadOnlyRepository storeReadOnlyRepository,
        ILoggedUserService loggedUserService,
        IUnitOfWork unitOfWork,
        IValidator<Communication.Request.SendOrderMessageRequestDto> validator)
    {
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _storeReadOnlyRepository = storeReadOnlyRepository;
        _loggedUserService = loggedUserService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<OrderResponseDto> Execute(Guid orderId, Communication.Request.SendOrderMessageRequestDto request)
    {
        await Validate(request);

        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        var currentUserId = _loggedUserService.GetUserId();
        var userRole = "User";

        var store = await _storeReadOnlyRepository.GetByIdAsync(order.StoreId);
        if (store is not null && store.UserId == currentUserId)
            userRole = "ShopKeeper";

        var messageType = request.Type == Communication.Enums.MessageTypeDto.Proposal
            ? Domain.Entities.Enums.MessageType.Proposal
            : Domain.Entities.Enums.MessageType.Text;

        var message = new Domain.Entities.OrderMessage
        {
            UserId = currentUserId,
            UserRole = userRole,
            Message = request.Message,
            Type = messageType,
            ProposedTotalAmount = request.ProposedTotalAmount,
            ProposedDeliveryDate = request.ProposedDeliveryDate
        };

        if (messageType == Domain.Entities.Enums.MessageType.Proposal)
        {
            order.Negotiate(request.ProposedTotalAmount, request.ProposedDeliveryDate, request.Message);
        }

        order.AddMessage(message);

        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        var response = await _orderWriteOnlyRepository.GetByIdTracking(orderId);
        return response.Adapt<OrderResponseDto>();
    }

    private async Task Validate(Communication.Request.SendOrderMessageRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
