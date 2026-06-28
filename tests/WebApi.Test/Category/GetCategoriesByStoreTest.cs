using IOrder.Communication.Response;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class GetCategoriesByStoreTest : IOrderClassFixture
{
    private readonly string method = "category/store";
    private readonly System.Guid _storeId;

    public GetCategoriesByStoreTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _storeId = factory.CategoryList.First().StoreId;
    }

    [Fact]
    public async Task Success()
    {
        var response = await DoGet($"{method}/{_storeId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<IList<CategoryResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData!.Count.ShouldBeGreaterThan(0);
    }
}
