using IOrder.Application.Services.Mapper;
using IOrder.Application.UseCases.Product;
using IOrder.Application.UseCases.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOrder.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddUseCase(services);
        MapsterSettings.Configure();
    }

    private static void AddUseCase(IServiceCollection services)
    {
        services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
        services.AddScoped<IGetProductsPaged, GetProductsPaged>();
        services.AddScoped<IDeleteProductUseCase, DeleteProductUseCase>();
        services.AddScoped<IGetProductById, GetProductById>();
        services.AddScoped<IUpdateProductUseCase, UpdateProductUseCase>();
        services.AddScoped<ICreatePromotionPriceUseCase, CreatePromotionPriceUseCase>();

        services.AddScoped<ICreateStoreUseCase, CreateStoreUseCase>();
        services.AddScoped<IDeleteStoreUseCase, DeleteStoreUseCase>();
        services.AddScoped<IUpdateStoreUseCase, UpdateStoreUseCase>();
        services.AddScoped<IUpdateAddressUseCase, UpdateAddressUseCase>();
        services.AddScoped<IUpdateOpeningHourUseCase, UpdateOpeningHourUseCase>();
        services.AddScoped<IGetAllStore, GetAllStore>();
        services.AddScoped<IGetByIdStoreUseCase, GetByIdStore>();
        services.AddScoped<IGetMyStoreUseCase, GetMyStoreUseCase>();
    }
}
