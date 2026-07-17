using IOrder.Communication.Response;
using Shouldly;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.StoreCategory;

public class GetAllStoreCategoryTest : IOrderClassFixture
{
    private readonly string method = "storeCategory";

    public GetAllStoreCategoryTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {

        var response = await DoGet(method);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<IList<StoreCategoryResponseDto>>();
        
        responseData.ShouldNotBeNull();
    }
}
