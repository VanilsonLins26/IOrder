using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IRequestEarlyDeliveryUseCase
{
    Task<OrderResponseDto> Execute(Guid orderId);
}

public class RequestEarlyDeliveryUseCase : IRequestEarlyDeliveryUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestEarlyDeliveryUseCase(
        ILoggedUserService loggedUserService,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponseDto> Execute(Guid orderId)
    {
        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        var userId = _loggedUserService.GetUserId();

        if (order.UserId != userId)
            throw new UnauthorizedStoreException([ResourceMessagesException.UNAUTHORIZED_STORE]);

        if (order.Status != Domain.Entities.Enums.OrderStatus.Ready)
            throw new ErrorOnValidationException(["O pedido precisa estar como 'Pronto' para solicitar entrega antecipada."]);

        if (order.RequestedEarlyDelivery)
            throw new ErrorOnValidationException(["Este pedido já possui uma solicitação de entrega antecipada."]);

        if (order.DeliveryType != Domain.Entities.Enums.DeliveryType.Delivery)
            throw new ErrorOnValidationException(["Solicitação de entrega antecipada disponível apenas para entregas."]);

        order.RequestEarlyDelivery();
        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        return order.Adapt<OrderResponseDto>();
    }
}
