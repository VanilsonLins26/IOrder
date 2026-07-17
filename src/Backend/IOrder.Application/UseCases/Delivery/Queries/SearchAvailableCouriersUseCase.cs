using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;

namespace IOrder.Application.UseCases.Delivery.Queries;

public interface ISearchAvailableCouriersUseCase
{
    Task<IReadOnlyList<AvailableCourierResponseDto>> Execute(Guid orderId);
}

public class SearchAvailableCouriersUseCase : ISearchAvailableCouriersUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IOrderReadOnlyRepository _orderReadOnlyRepository;
    private readonly ICourierLocationReadOnlyRepository _courierLocationRepository;

    public SearchAvailableCouriersUseCase(
        ILoggedUserService loggedUserService,
        IStorePermissionService storePermissionService,
        IOrderReadOnlyRepository orderReadOnlyRepository,
        ICourierLocationReadOnlyRepository courierLocationRepository)
    {
        _loggedUserService = loggedUserService;
        _storePermissionService = storePermissionService;
        _orderReadOnlyRepository = orderReadOnlyRepository;
        _courierLocationRepository = courierLocationRepository;
    }

    public async Task<IReadOnlyList<AvailableCourierResponseDto>> Execute(Guid orderId)
    {
        var order = await _orderReadOnlyRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException([ResourceMessagesException.ORDER_NOT_FOUND]);

        await _storePermissionService.ValidateStoreOwnerAsync(order.StoreId);

        if (order.Store == null || order.Store.Location == null)
            throw new ErrorOnValidationException(["Loja sem localização configurada."]);

        if (order.Store.DeliveryPartner != Domain.Entities.Enums.DeliveryPartner.App)
            throw new ErrorOnValidationException(["Busca de entregadores disponível apenas para entregas do app."]);

        var storeLat = order.Store.Location.Y;
        var storeLon = order.Store.Location.X;

        var available = await _courierLocationRepository.GetAvailableCouriersAsync(
            storeLat, storeLon, 15.0, order.Id);

        return available.Select(c => new AvailableCourierResponseDto
        {
            CourierUserId = c.CourierUserId,
            DistanceKm = c.DistanceKm,
            LastLocationAt = c.LastLocationAt
        }).ToList();
    }
}
