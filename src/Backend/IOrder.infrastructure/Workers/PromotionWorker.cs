using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Workers;

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
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var agora = DateTime.UtcNow;

                var promocoesParaIniciar = await dbContext.Promotions
                    .Include(p => p.Product)
                    .Where(p => p.Active && p.InitialTime <= agora && p.FinalTime >= agora
                             && p.Product.CurrentPromotionalPrice == null)
                    .ToListAsync(stoppingToken);
                foreach (var promo in promocoesParaIniciar)
                {

                    promo.Product.SetCurrentPromotionalPrice(promo.Price);
                }
                var promocoesVencidas = await dbContext.Promotions
                    .Include(p => p.Product)
                    .Where(p => p.FinalTime < agora && p.Product.CurrentPromotionalPrice != null)
                    .ToListAsync(stoppingToken);
                foreach (var promo in promocoesVencidas)
                {
                    promo.Product.RemovePromotionalPrice(); 
                    promo.Active = false; 
                }

                if (promocoesParaIniciar.Any() || promocoesVencidas.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
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