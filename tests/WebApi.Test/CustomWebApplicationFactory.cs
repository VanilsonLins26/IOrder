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
using Testcontainers.Redis;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;
    private readonly RedisContainer _redisContainer;
    public IEnumerable<IOrder.Domain.Entities.Product> ProductList { get; private set; } = [];
    public IEnumerable<IOrder.Domain.Entities.Category> CategoryList { get; private set; } = [];

    public CustomWebApplicationFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0")
                              .WithDatabase("iorder")
                              .Build();

        _redisContainer = new RedisBuilder()
                              .WithImage("redis:7.0")
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



            services.AddAuthentication("Test")
                    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { });

            services.Configure<Microsoft.AspNetCore.Authentication.AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(_mySqlContainer.GetConnectionString(), serverVersion));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
            });
        });
    }
    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mySqlContainer.StartAsync(), _redisContainer.StartAsync());

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        var category = await dbContext.StoreCategories.FirstOrDefaultAsync();

        var store = new IOrder.Domain.Entities.Store
        {
            Name = "Test Store",
            UserId = "test-user-123",
            ImageUrl = "test.png",
            CategoryId = category!.Id
        };
        await dbContext.Stores.AddAsync(store);

        IOrder.Domain.Entities.Product product1 = ProductBuilder.Build();
        product1.StoreId = store.Id;

        IOrder.Domain.Entities.Product product2 = ProductBuilder.Build();
        product2.StoreId = store.Id;

        IOrder.Domain.Entities.Product product3 = ProductBuilder.Build();
        product3.StoreId = store.Id;

        ProductList = [product1, product2, product3];
        await dbContext.Products.AddRangeAsync(product1, product2, product3);

        var category1 = CategoryBuilder.Build(store.Id);
        var category2 = CategoryBuilder.Build(store.Id);
        CategoryList = [category1, category2];
        await dbContext.Categories.AddRangeAsync(category1, category2);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(_mySqlContainer.DisposeAsync().AsTask(), _redisContainer.DisposeAsync().AsTask());
    }
}
