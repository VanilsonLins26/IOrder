using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Security.Services;
using Mapster;

namespace IOrder.Application.UseCases.Delivery.Queries;

public interface IGetMyDeliveriesUseCase
{
    Task<PagedResponse<DeliveryAssignmentResponseDto>> Execute(int pageNumber, int pageSize);
}

public class GetMyDeliveriesUseCase : IGetMyDeliveriesUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IDeliveryAssignmentReadOnlyRepository _readOnlyRepository;

    public GetMyDeliveriesUseCase(
        ILoggedUserService loggedUserService,
        IDeliveryAssignmentReadOnlyRepository readOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedResponse<DeliveryAssignmentResponseDto>> Execute(int pageNumber, int pageSize)
    {
        var userId = _loggedUserService.GetUserId();
        var assignments = await _readOnlyRepository.GetByCourierUserIdAsync(userId, pageNumber, pageSize);
        var totalCount = await _readOnlyRepository.GetCountByCourierUserIdAsync(userId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse<DeliveryAssignmentResponseDto>
        {
            Items = assignments.Adapt<List<DeliveryAssignmentResponseDto>>(),
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }
}
