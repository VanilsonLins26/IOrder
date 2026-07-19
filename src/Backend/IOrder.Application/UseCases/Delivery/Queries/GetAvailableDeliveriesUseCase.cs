using IOrder.Domain.Security.Services;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;

namespace IOrder.Application.UseCases.Delivery.Queries;

public interface IGetAvailableDeliveriesUseCase
{
    Task<PagedResponse<AvailableDeliveryResponseDto>> ExecuteAsync();
}

public class GetAvailableDeliveriesUseCase : IGetAvailableDeliveriesUseCase
{
    private readonly ILoggedUserService _loggedUser;
    private readonly IOrderReadOnlyRepository _orderRepository;
    private readonly ICourierLocationReadOnlyRepository _locationRepository;

    public GetAvailableDeliveriesUseCase(
        ILoggedUserService loggedUser,
        IOrderReadOnlyRepository orderRepository,
        ICourierLocationReadOnlyRepository locationRepository)
    {
        _loggedUser = loggedUser;
        _orderRepository = orderRepository;
        _locationRepository = locationRepository;
    }

    public async Task<PagedResponse<AvailableDeliveryResponseDto>> ExecuteAsync()
    {
        var userId = _loggedUser.GetUserId();
        var location = await _locationRepository.GetByCourierUserIdAsync(userId);
        
        if (location == null)
        {
            // For portfolio testing purposes, if courier doesn't have a location yet,
            // we will pretend they are at a default location in Fortaleza (or store's city)
            location = new CourierLocation 
            { 
                CourierUserId = userId,
                Location = new NetTopologySuite.Geometries.Point(-38.5267, -3.7319) { SRID = 4326 }
            };
        }

        var maxDistanceKm = 99999.0; // Ignore distance for portfolio test
        var availableOrders = await _orderRepository.GetAvailableForDeliveryAsync(location.Location.Y, location.Location.X, maxDistanceKm);

        var items = availableOrders.Select(o => 
        {
            var distance = o.Store?.Location?.Distance(location.Location) ?? 0;
            // Coordinate distance to km approx: degrees * 111
            var distanceKm = distance * 111.0;

            return new AvailableDeliveryResponseDto
            {
                OrderId = o.Id,
                StoreName = o.Store?.Name ?? string.Empty,
                StoreImageUrl = o.Store?.ImageUrl ?? string.Empty,
                StoreAddress = o.Store?.Address != null 
                    ? $"{o.Store.Address.Street}, {o.Store.Address.Number} - {o.Store.Address.Neighborhood}" 
                    : string.Empty,
                DeliveryFee = o.DeliveryFee,
                DistanceKm = Math.Round(distanceKm, 1),
                ClientDistanceKm = 3.2, // Mock client distance
                DeliveryDate = o.DeliveryDate,
                DeliveryDateEnd = o.DeliveryDate?.AddMinutes(30),
                RequestedEarlyDelivery = o.RequestedEarlyDelivery,
                CreatedAt = o.CreatedAt
            };
        }).OrderBy(x => x.DistanceKm).ToList();

        return new PagedResponse<AvailableDeliveryResponseDto>(items, items.Count, 1, items.Count == 0 ? 1 : items.Count);
    }
}
