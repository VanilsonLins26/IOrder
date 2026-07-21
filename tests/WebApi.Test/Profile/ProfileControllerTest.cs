using CommomTestUtilities.Requests.Profile;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace WebApi.Test.Profile;

public class ProfileControllerTest : IOrderClassFixture
{
    private readonly string method = "profile";
    private readonly string _userToken = Guid.NewGuid().ToString();

    public ProfileControllerTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_Success()
    {
        var response = await DoGet(method, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserProfileResponseDto>();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Update_Success()
    {
        var request = new UserProfileRequestDto
        {
            Email = "test@example.com",
            Phone = "11999999999"
        };

        var response = await DoPut(method, request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserProfileResponseDto>();
        result.ShouldNotBeNull();
        result.Email.ShouldBe(request.Email);
        result.Phone.ShouldBe(request.Phone);
    }
}
