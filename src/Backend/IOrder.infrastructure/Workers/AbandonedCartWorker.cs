using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using IOrder.Domain.Entities;
using IOrder.Domain.Events;
using IOrder.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IOrder.infrastructure.Workers;

[ExcludeFromCodeCoverage]
public class AbandonedCartWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AbandonedCartWorker> _logger;
    private readonly TimeSpan _abandonmentThreshold = TimeSpan.FromMinutes(30);

    public AbandonedCartWorker(IServiceScopeFactory scopeFactory, ILogger<AbandonedCartWorker> logger)
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
                var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

                var server = redis.GetServer(redis.GetEndPoints().First());
                var cutoff = DateTime.UtcNow - _abandonmentThreshold;

                foreach (var key in server.Keys(pattern: "cart:*", pageSize: 100))
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    var redisDb = redis.GetDatabase();
                    var cartJson = (string?)await redisDb.StringGetAsync(key);

                    if (string.IsNullOrEmpty(cartJson)) continue;

                    var cart = JsonSerializer.Deserialize<Cart>(cartJson);
                    if (cart is null || cart.Items.Count == 0) continue;

                    if (cart.LastModifiedAt < cutoff)
                    {
                        _logger.LogInformation(
                            "Cart abandoned by user {UserId} (last modified: {LastModifiedAt})",
                            cart.UserId, cart.LastModifiedAt);

                        var userPhone = cart.UserPhone;
                        if (string.IsNullOrEmpty(userPhone))
                        {
                            _logger.LogWarning("User {UserId} has no phone — skipping abandoned cart event", cart.UserId);
                            continue;
                        }

                        await dispatcher.DispatchAsync([new CartAbandonedEvent(cart.UserId, userPhone)]);

                        await redisDb.KeyDeleteAsync(key);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking abandoned carts");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
