using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IRejectDeliveryUseCase
{
    Task<DeliveryAssignmentResponseDto> Execute(Guid assignmentId);
}

public class RejectDeliveryUseCase : IRejectDeliveryUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IDeliveryAssignmentReadOnlyRepository _readOnlyRepository;
    private readonly IDeliveryAssignmentWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectDeliveryUseCase(
        ILoggedUserService loggedUserService,
        IDeliveryAssignmentReadOnlyRepository readOnlyRepository,
        IDeliveryAssignmentWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryAssignmentResponseDto> Execute(Guid assignmentId)
    {
        var userId = _loggedUserService.GetUserId();

        var assignment = await _writeOnlyRepository.GetByIdTrackingAsync(assignmentId)
            ?? throw new NotFoundException(["Atribuição não encontrada."]);

        if (assignment.CourierUserId != userId)
            throw new UnauthorizedStoreException(["Você não tem permissão para rejeitar esta atribuição."]);

        if (assignment.Status != Domain.Entities.Enums.AssignmentStatus.Pending)
            throw new ErrorOnValidationException(["Esta atribuição não está pendente."]);

        assignment.Reject();
        _writeOnlyRepository.Update(assignment);
        await _unitOfWork.Commit();

        return assignment.Adapt<DeliveryAssignmentResponseDto>();
    }
}
