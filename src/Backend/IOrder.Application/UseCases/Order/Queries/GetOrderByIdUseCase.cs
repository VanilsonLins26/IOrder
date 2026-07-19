using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Order.Queries;

public class GetOrderByIdUseCase : IGetOrderByIdUseCase
{
    private readonly IOrderReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;

    public GetOrderByIdUseCase(
        IOrderReadOnlyRepository readOnlyRepository,
        ILoggedUserService loggedUserService)
    {
        _readOnlyRepository = readOnlyRepository;
        _loggedUserService = loggedUserService;
    }

    public async Task<OrderResponseDto> Execute(Guid id)
    {
        var userId = _loggedUserService.GetUserId()
            ?? throw new UnauthorizedStoreException([ResourceMessagesException.ORDER_NOT_FOUND]);
        var order = await _readOnlyRepository.GetByIdAsync(id)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        var isAssignedCourier = order.Assignments.Any(a => 
            a.CourierUserId == userId && 
            a.Status != Domain.Entities.Enums.AssignmentStatus.Rejected && 
            a.Status != Domain.Entities.Enums.AssignmentStatus.Failed);

        if (order.UserId != userId && order.Store?.UserId != userId && !isAssignedCourier)
            throw new UnauthorizedStoreException([ResourceMessagesException.ORDER_NOT_FOUND]);

        return order.Adapt<OrderResponseDto>();
    }
}
