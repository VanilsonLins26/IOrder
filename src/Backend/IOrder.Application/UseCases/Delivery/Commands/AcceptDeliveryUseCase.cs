using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IAcceptDeliveryUseCase
{
    Task<DeliveryAssignmentResponseDto> Execute(Guid orderId);
}

public class AcceptDeliveryUseCase : IAcceptDeliveryUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IDeliveryAssignmentWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptDeliveryUseCase(
        ILoggedUserService loggedUserService,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IDeliveryAssignmentWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryAssignmentResponseDto> Execute(Guid orderId)
    {
        var userId = _loggedUserService.GetUserId();

        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException(["Pedido não encontrado."]);

        if (!order.IsSearchingCourier)
            throw new ErrorOnValidationException(["Este pedido não está buscando entregadores."]);

        if (order.Assignments.Any(a => 
            a.Status != Domain.Entities.Enums.AssignmentStatus.Rejected 
            && a.Status != Domain.Entities.Enums.AssignmentStatus.Failed))
        {
            throw new ErrorOnValidationException(["Esta entrega já foi aceita por outro entregador."]);
        }

        var assignment = new Domain.Entities.DeliveryAssignment
        {
            OrderId = orderId,
            CourierUserId = userId
        };

        // Instantly accept
        assignment.Accept();

        await _writeOnlyRepository.CreateAsync(assignment);
        order.AssignCourier(assignment);
        order.StopSearchingCourier();
        
        // Status can move to Preparing if it was Ready/Preparing? Wait, typically it moves to Preparing? Or if it was Ready it stays Ready?
        // Let's keep it as is, or we just leave the status untouched and let the store manage it.
        // Actually, Courier acceptance might not change the food status. But let's leave it as is or move to Preparing.

        _orderWriteOnlyRepository.Update(order);
        await _unitOfWork.Commit();

        return assignment.Adapt<DeliveryAssignmentResponseDto>();
    }
}
