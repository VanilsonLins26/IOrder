using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;

namespace IOrder.infrastructure.Workers;

[ExcludeFromCodeCoverage]
public class PromotionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PromotionWorker> _logger;
    public PromotionWorker(IServiceScopeFactory scopeFactory, ILogger<PromotionWorker> logger)
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
                var repository = scope.ServiceProvider.GetRequiredService<IProductWriteOnlyRepository>();
                var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var agora = DateTime.UtcNow;

                var promocoesParaIniciar = await repository.GetPromotionsToStartAsync(agora, stoppingToken);
                foreach (var promo in promocoesParaIniciar)
                {
                    promo.Product.SetCurrentPromotionalPrice(promo.Price);
                }

                var promocoesVencidas = await repository.GetPromotionsToFinishAsync(agora, stoppingToken);
                foreach (var promo in promocoesVencidas)
                {
                    promo.Product.RemovePromotionalPrice(); 
                    promo.Deactivate(); 
                }

                if (promocoesParaIniciar.Any() || promocoesVencidas.Any())
                {
                    await uof.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}