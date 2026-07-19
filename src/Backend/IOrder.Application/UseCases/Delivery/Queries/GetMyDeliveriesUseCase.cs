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
    private readonly ICourierLocationReadOnlyRepository _locationRepository;

    public GetMyDeliveriesUseCase(
        ILoggedUserService loggedUserService,
        IDeliveryAssignmentReadOnlyRepository readOnlyRepository,
        ICourierLocationReadOnlyRepository locationRepository)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepository = readOnlyRepository;
        _locationRepository = locationRepository;
    }

    public async Task<PagedResponse<DeliveryAssignmentResponseDto>> Execute(int pageNumber, int pageSize)
    {
        var userId = _loggedUserService.GetUserId();
        var assignments = await _readOnlyRepository.GetByCourierUserIdAsync(userId, pageNumber, pageSize);
        var totalCount = await _readOnlyRepository.GetCountByCourierUserIdAsync(userId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var location = await _locationRepository.GetByCourierUserIdAsync(userId);

        var items = assignments.Select(a => 
        {
            var dto = a.Adapt<DeliveryAssignmentResponseDto>();
            dto.StoreName = a.Order?.Store?.Name ?? string.Empty;
            dto.ClientDistanceKm = 3.2; // Mock distance since client address is not in DB yet
            
            if (location != null && a.Order?.Store?.Location != null)
            {
                var distance = a.Order.Store.Location.Distance(location.Location);
                dto.DistanceKm = Math.Round(distance * 111.0, 1);
            }
            else
            {
                dto.DistanceKm = 4.5; // fallback
            }

            return dto;
        }).ToList();

        return new PagedResponse<DeliveryAssignmentResponseDto>
        {
            Items = items,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }
}
