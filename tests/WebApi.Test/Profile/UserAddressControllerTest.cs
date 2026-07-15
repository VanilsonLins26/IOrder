using CommomTestUtilities.Requests.Profile;
using IOrder.Communication.Response.Profile;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace WebApi.Test.Profile;

public class UserAddressControllerTest : IOrderClassFixture
{
    private readonly string method = "useraddress";

    public UserAddressControllerTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Add_Success()
    {
        var request = AddUserAddressRequestBuilder.Build();

        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<UserAddressResponseDto>();
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(request.Name);
    }

    [Fact]
    public async Task GetAll_Success()
    {
        var response = await DoGet(method);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<UserAddressResponseDto>>();
        result.ShouldNotBeNull();
    }
}
