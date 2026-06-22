using IOrder.Application.Services.LoggedUser;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.infrastructure.DataAccess;
using IOrder.infrastructure.Repositories;
using IOrder.infrastructure.Repositories.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOrder.infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
        AddServices(services);
        AddWorkers(services);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

        services.AddDbContext<AppDbContext>(config => config.UseMySql(connectionString, serverVersion));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IProductWriteOnlyRepository, ProductRepository>();
        services.AddScoped<IProductReadOnlyRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ILoggedUserService, LoggedUser.LoggedUserService>();
    }

    private static void AddWorkers(IServiceCollection services)
    {
        services.AddHostedService<Workers.PromotionWorker>();
    }

    public static async Task MigrateDatabaseAsync(this Microsoft.AspNetCore.Builder.IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
