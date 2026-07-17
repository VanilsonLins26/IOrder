using IOrder.Communication.Response;
using IOrder.Exceptions;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Store;

public class GetMyStoreTest : IOrderClassFixture
{
    private readonly string method = "store/myStore";

    public GetMyStoreTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var token = "test-user-123";
        var response = await DoGet(method, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<StoreResponseDto>();
        responseData.ShouldNotBeNull();
        responseData!.Name.ShouldBe("Test Store");
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var token = "test-new-user";
        var response = await DoGet(method, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.STORE_NOT_FOUND);
    }
}
