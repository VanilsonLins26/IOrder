using CommomTestUtilities.Requests.Store;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using System;

namespace WebApi.Test.Store;

public class UpdateOpeningHourTest : IOrderClassFixture
{
    private readonly string method = "store/openingHour";
    private readonly AppDbContext _dbContext;

    public UpdateOpeningHourTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var store = await _dbContext.Stores.FirstAsync();
        var request = UpdateOpeningHourRequestBuilder.Build();
        
        var token = "test-user-123";
        var response = await DoPut($"{method}/{store.Id}", request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var request = UpdateOpeningHourRequestBuilder.Build();
        var token = "test-user-123";
        var response = await DoPut($"{method}/{Guid.NewGuid()}", request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.STORE_NOT_FOUND);
    }
}
