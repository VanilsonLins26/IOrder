using System.Diagnostics.CodeAnalysis;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Delivery;
using IOrder.Domain.Repositories.Order;
using IOrder.infrastructure.Hubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace IOrder.infrastructure.Workers;

[ExcludeFromCodeCoverage]
public class CourierAutoSearchService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CourierAutoSearchService> _logger;

    public CourierAutoSearchService(
        IServiceScopeFactory scopeFactory,
        ILogger<CourierAutoSearchService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderReadOnlyRepository>();
                var courierLocationRepo = scope.ServiceProvider.GetRequiredService<ICourierLocationReadOnlyRepository>();
                var deliveryWriteRepo = scope.ServiceProvider.GetRequiredService<IDeliveryAssignmentWriteOnlyRepository>();
                var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();

                var eligibleOrders = await orderRepo.GetEligibleForAutoSearchAsync();

                foreach (var order in eligibleOrders)
                {
                    if (order.Store == null || order.Store.Location == null) continue;

                    var storeLat = order.Store.Location.Y;
                    var storeLon = order.Store.Location.X;

                    if (!order.IsSearchingCourier)
                    {
                        order.StartSearchingCourier();
                        
                        _logger.LogInformation(
                            "Auto-search: broadcasting offer for order {Order}",
                            order.Id);

                        await hubContext.Clients.Group("Couriers").SendAsync(
                            "NewDeliveryAvailable");
                    }
                }
                
                await uof.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CourierAutoSearchService");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
