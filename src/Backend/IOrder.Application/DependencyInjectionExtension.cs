using IOrder.Application.Services.Mapper;
using IOrder.Application.UseCases.Category.Commands;
using IOrder.Application.UseCases.Category.Queries;
using IOrder.Application.UseCases.Product.Commands;
using IOrder.Application.UseCases.Product.Queries;
using IOrder.Application.UseCases.Store.Commands;
using IOrder.Application.UseCases.Store.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace IOrder.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddUseCase(services);
        services.AddScoped<IOrder.Application.Services.StorePermission.IStorePermissionService, IOrder.Application.Services.StorePermission.StorePermissionService>();
        MapsterSettings.Configure();
    }

    private static void AddUseCase(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var assembly = Assembly.GetExecutingAssembly();
        
        var useCaseTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("UseCase"))
            .ToList();

        foreach (var type in useCaseTypes)
        {
            var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }
    }
}

