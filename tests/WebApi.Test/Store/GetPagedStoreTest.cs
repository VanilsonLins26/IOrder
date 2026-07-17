using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Repositories.Product;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Store;

public class GetPagedStoreTest : IOrderClassFixture
{
    private readonly string method = "store/paged";

    public GetPagedStoreTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var response = await DoGet(method);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<StoreResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData!.Items.ShouldNotBeEmpty();
    }
}
