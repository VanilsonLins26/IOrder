using FluentValidation;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Application.Services.StorePermission;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Commands;

public interface IAssignDeliveryUseCase
{
    Task<DeliveryAssignmentResponseDto> Execute(Guid orderId, AssignCourierRequestDto request);
}

public class AssignDeliveryUseCase : IAssignDeliveryUseCase
{
    private readonly IValidator<AssignCourierRequestDto> _validator;
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IOrderWriteOnlyRepository _orderWriteOnlyRepository;
    private readonly IDeliveryAssignmentReadOnlyRepository _readOnlyRepository;
    private readonly IDeliveryAssignmentWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignDeliveryUseCase(
        IValidator<AssignCourierRequestDto> validator,
        ILoggedUserService loggedUserService,
        IStorePermissionService storePermissionService,
        IOrderWriteOnlyRepository orderWriteOnlyRepository,
        IDeliveryAssignmentReadOnlyRepository readOnlyRepository,
        IDeliveryAssignmentWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
        _orderWriteOnlyRepository = orderWriteOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryAssignmentResponseDto> Execute(Guid orderId, AssignCourierRequestDto request)
    {
        await Validate(request);

        var order = await _orderWriteOnlyRepository.GetByIdTracking(orderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);

        var existingAssignment = await _readOnlyRepository.GetByOrderIdAsync(orderId);
        if (existingAssignment is not null &&
            existingAssignment.Status != Domain.Entities.Enums.AssignmentStatus.Rejected &&
            existingAssignment.Status != Domain.Entities.Enums.AssignmentStatus.Failed)
        {
            throw new ErrorOnValidationException(["Este pedido já possui um entregador atribuído."]);
        }

        var assignment = new Domain.Entities.DeliveryAssignment
        {
            OrderId = orderId,
            CourierUserId = request.CourierUserId
        };

        order.AssignCourier(assignment);
        await _writeOnlyRepository.CreateAsync(assignment);
        await _unitOfWork.Commit();

        return assignment.Adapt<DeliveryAssignmentResponseDto>();
    }

    private async Task Validate(AssignCourierRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
