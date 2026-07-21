using CommomTestUtilities.Requests;
using IOrder.Communication.Request;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Delivery;

public class DeliveryIntegrationTest : IOrderClassFixture
{
    private readonly string method = "delivery";
    private readonly string _userToken = System.Guid.NewGuid().ToString();

    public DeliveryIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_MyDeliveries_Returns_Ok()
    {
        var response = await DoGet($"{method}/my-deliveries", _userToken);
        
        // It might be Empty list, but should be OK
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Put_UpdateLocation_Returns_NoContent()
    {
        var request = UpdateCourierLocationRequestBuilder.Build();
        
        var response = await DoPut($"{method}/location", request, _userToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
