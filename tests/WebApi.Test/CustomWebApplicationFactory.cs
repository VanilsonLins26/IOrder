using CommomTestUtilities.Entities;
using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testcontainers.MySql;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;
    public IEnumerable<IOrder.Domain.Entities.Product> ProductList { get; private set; } = [];

    public CustomWebApplicationFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0")
                              .WithDatabase("iorder")
                              .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));


            if (descriptor != null)
                services.Remove(descriptor);

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(_mySqlContainer.GetConnectionString(), serverVersion));
        });
    }
    public async Task InitializeAsync()
    {

        await _mySqlContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync();


        IOrder.Domain.Entities.Product product1 = ProductBuilder.Build();
        IOrder.Domain.Entities.Product product2 = ProductBuilder.Build();
        IOrder.Domain.Entities.Product product3 = ProductBuilder.Build();

        ProductList = [product1, product2, product3];

        await dbContext.Products.AddRangeAsync(product1, product2, product3);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _mySqlContainer.DisposeAsync();
    }
}
