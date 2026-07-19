using IOrder.Application.Services.Payment;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Payment;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Domain.Services;
using IOrder.infrastructure.DataAccess;
using IOrder.infrastructure.Repositories;
using IOrder.infrastructure.Repositories.Payment;
using IOrder.infrastructure.Repositories.Product;
using IOrder.infrastructure.Repositories.Store;
using IOrder.infrastructure.DataAccess.Repositories;
using IOrder.infrastructure.Services;
using IOrder.infrastructure.Services.Email;
using IOrder.infrastructure.Services.Evolution;
using IOrder.infrastructure.Services.MessageBus;
using IOrder.infrastructure.Services.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IOrder.Domain.Repositories.Cart;
using IOrder.infrastructure.Repositories.Cart;
using IOrder.Domain.Repositories.Coupon;
using IOrder.infrastructure.Repositories.Coupon;
using IOrder.Domain.Repositories.Order;
using IOrder.infrastructure.Repositories.Order;
using IOrder.Domain.Repositories.Profile;
using IOrder.infrastructure.Repositories.Profile;
using IOrder.Domain.Repositories.Customization;
using IOrder.infrastructure.Repositories.Customization;
using IOrder.Domain.Repositories.Delivery;
using IOrder.infrastructure.Repositories.Delivery;
using IOrder.Domain.Repositories.Review;
using IOrder.infrastructure.Repositories.Review;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IOrder.infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddRepositories(services);
        AddServices(services, configuration);
        AddWorkers(services);
        AddRedisCache(services, configuration);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

        services.AddDbContext<AppDbContext>(config => 
            config.UseMySql(connectionString, serverVersion, x => x.UseNetTopologySuite()));
    }

    private static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnection");
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IProductWriteOnlyRepository, ProductRepository>();
        services.AddScoped<IProductReadOnlyRepository, ProductRepository>();

        services.AddScoped<IStoreWriteOnlyRepository, StoreRepository>();
        services.AddScoped<IStoreReadOnlyRepository, StoreRepository>();

        services.AddScoped<IOrder.Domain.Repositories.Category.ICategoryWriteOnlyRepository, IOrder.infrastructure.Repositories.Category.CategoryRepository>();
        services.AddScoped<IOrder.Domain.Repositories.Category.ICategoryReadOnlyRepository, IOrder.infrastructure.Repositories.Category.CategoryRepository>();

        services.AddScoped<IOrder.Domain.Repositories.StoreCategory.IStoreCategoryReadOnlyRepository, IOrder.infrastructure.Repositories.StoreCategory.StoreCategoryRepository>();

        services.AddScoped<ICartWriteOnlyRepository, CartRepository>();
        services.AddScoped<ICartReadOnlyRepository, CartRepository>();

        services.AddScoped<ICouponWriteOnlyRepository, CouponRepository>();
        services.AddScoped<ICouponReadOnlyRepository, CouponRepository>();

        services.AddScoped<IOrderWriteOnlyRepository, OrderRepository>();
        services.AddScoped<IOrderReadOnlyRepository, OrderRepository>();
        services.AddScoped<IChatReadOnlyRepository, OrderRepository>();

        services.AddScoped<IProfileReadOnlyRepository, ProfileRepository>();
        services.AddScoped<IProfileWriteOnlyRepository, ProfileRepository>();

        services.AddScoped<ICustomizationReadOnlyRepository, CustomizationRepository>();
        services.AddScoped<ICustomizationWriteOnlyRepository, CustomizationRepository>();

        services.AddScoped<IPaymentReadOnlyRepository, PaymentRepository>();
        services.AddScoped<IPaymentWriteOnlyRepository, PaymentRepository>();

        services.AddScoped<IUserCardReadOnlyRepository, UserCardRepository>();
        services.AddScoped<IUserCardWriteOnlyRepository, UserCardRepository>();

        services.AddScoped<IUserAddressReadOnlyRepository, UserAddressRepository>();
        services.AddScoped<IUserAddressWriteOnlyRepository, UserAddressRepository>();

        services.AddScoped<IDeliveryAssignmentReadOnlyRepository, DeliveryAssignmentRepository>();
        services.AddScoped<IDeliveryAssignmentWriteOnlyRepository, DeliveryAssignmentRepository>();

        services.AddScoped<ICourierLocationReadOnlyRepository, CourierLocationRepository>();
        services.AddScoped<ICourierLocationWriteOnlyRepository, CourierLocationRepository>();

        services.AddScoped<IReviewReadOnlyRepository, ReviewRepository>();
        services.AddScoped<IReviewWriteOnlyRepository, ReviewRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ILoggedUserService, LoggedUser.LoggedUserService>();
        services.AddScoped<IStorageService, CloudinaryStorageService>();
        services.AddSingleton<RabbitMQConnectionFactory>();
        services.AddScoped<IOrderMessagePublisher, RabbitMQMessagePublisher>();
        services.AddSingleton<KafkaProducerFactory>();
        services.AddScoped<IDomainEventDispatcher, KafkaDomainEventDispatcher>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEvolutionApiService, EvolutionApiService>();
        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.SectionName));
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddHttpClient<IGeocodingService, IOrder.infrastructure.Services.Geocoding.NominatimGeocodingService>();
    }

    private static void AddWorkers(IServiceCollection services)
    {
        services.AddHostedService<Workers.PromotionWorker>();
        services.AddHostedService<Workers.ChatConsumer>();
        services.AddHostedService<Workers.KafkaDomainEventConsumer>();
        services.AddHostedService<Workers.AbandonedCartWorker>();
        services.AddHostedService<Workers.CourierAutoSearchService>();
    }

    public static async Task MigrateDatabaseAsync(this Microsoft.AspNetCore.Builder.IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
