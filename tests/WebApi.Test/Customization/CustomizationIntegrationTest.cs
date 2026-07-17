using CommomTestUtilities.Requests.Customization;
using IOrder.Communication.Response;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using IOrder.Domain.Entities;
using IOrder.Communication.Request;

namespace WebApi.Test.Customization;

public class CustomizationIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _adminToken = "test-user-123";

    public CustomizationIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateGroup_Success()
    {
        var product = _factory.ProductList.First();

        var request = SaveCustomizationGroupRequestBuilder.Build();

        var response = await DoPost($"customization/product/{product.Id}", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<CustomizationGroupResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Name.ShouldBe(request.Name);
        responseData.Required.ShouldBe(request.Required);
        responseData.Options.Count.ShouldBe(request.Options.Count);
    }

    [Fact]
    public async Task GetByProduct_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = _factory.ProductList.Last();
        
        var group = new CustomizationGroup
        {
            ProductId = product.Id,
            Name = "Group Test",
            Required = true
        };
        group.AddOption(new CustomizationOption { Name = "Option 1", PriceModifier = 10 });
        group.AddOption(new CustomizationOption { Name = "Option 2", PriceModifier = 20 });

        await dbContext.Set<CustomizationGroup>().AddAsync(group);
        await dbContext.SaveChangesAsync();

        var response = await DoGet($"customization/product/{product.Id}"); // public access

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<List<CustomizationGroupResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.ShouldNotBeEmpty();
        responseData.First().Name.ShouldBe(group.Name);
    }

    [Fact]
    public async Task DeleteGroup_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = _factory.ProductList.First();

        var group = new CustomizationGroup
        {
            ProductId = product.Id,
            Name = "Group To Delete",
            Required = false
        };

        await dbContext.Set<CustomizationGroup>().AddAsync(group);
        await dbContext.SaveChangesAsync();

        var response = await DoDelete($"customization/{group.Id}", _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        dbContext.ChangeTracker.Clear();
        var deletedGroup = await dbContext.Set<CustomizationGroup>().FindAsync(group.Id);
        deletedGroup.ShouldBeNull();
    }
}
