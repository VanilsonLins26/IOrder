using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface ICourierStartTransitUseCase
{
    Task<DeliveryAssignmentResponseDto> Execute(Guid assignmentId);
}

public class CourierStartTransitUseCase : ICourierStartTransitUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IDeliveryAssignmentWriteOnlyRepository _writeOnlyRepository;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourierStartTransitUseCase(
        ILoggedUserService loggedUserService,
        IDeliveryAssignmentWriteOnlyRepository writeOnlyRepository,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _writeOnlyRepository = writeOnlyRepository;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryAssignmentResponseDto> Execute(Guid assignmentId)
    {
        var userId = _loggedUserService.GetUserId();

        var assignment = await _writeOnlyRepository.GetByIdTrackingAsync(assignmentId)
            ?? throw new NotFoundException(["Atribuição não encontrada."]);

        if (assignment.CourierUserId != userId)
            throw new UnauthorizedStoreException(["Você não tem permissão para iniciar o trânsito."]);

        if (assignment.Status != Domain.Entities.Enums.AssignmentStatus.PickedUp)
            throw new ErrorOnValidationException(["O pedido precisa ser coletado antes de sair para entrega."]);

        assignment.StartTransit();
        _writeOnlyRepository.Update(assignment);

        var order = await _orderWriteOnlyRepository.GetByIdTracking(assignment.OrderId);
        if (order is not null)
        {
            order.MarkAsOutForDelivery();
            _orderWriteOnlyRepository.Update(order);
        }

        await _unitOfWork.Commit();

        return assignment.Adapt<DeliveryAssignmentResponseDto>();
    }
}
