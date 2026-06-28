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
        // StoreCategory uses AllowAnonymous essentially (Controller has no [Authorize])
        var response = await DoGet(method);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<IList<StoreCategoryResponseDto>>();
        
        // Since database may or may not be seeded with store categories depending on the factory, 
        // we just assert it returns a valid list.
        responseData.ShouldNotBeNull();
    }
}
