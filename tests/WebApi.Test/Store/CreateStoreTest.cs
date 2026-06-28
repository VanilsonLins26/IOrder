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

namespace WebApi.Test.Store;

public class CreateStoreTest : IOrderClassFixture
{
    private readonly string method = "store";
    private readonly AppDbContext _dbContext;

    public CreateStoreTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var request = StoreRequestBuilder.Build();
        request.CategoryId = _dbContext.StoreCategories.First().Id;

        var token = "test-new-user";
        var response = await DoPost(method, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<StoreResponseDto>();
        responseData.ShouldNotBeNull();
        responseData!.Name.ShouldBe(request.Name);
    }

    [Fact]
    public async Task Error_User_Already_Has_Store()
    {
        var request = StoreRequestBuilder.Build();
        request.CategoryId = _dbContext.StoreCategories.First().Id;

        var token = "test-user-123";
        var response = await DoPost(method, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.USER_ALREADY_HAS_STORE);
    }
}
