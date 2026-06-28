using CommomTestUtilities.Requests.Category;
using IOrder.Exceptions;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class UpdateCategoryPositionsTest : IOrderClassFixture
{
    private readonly string method = "category/positions";

    public UpdateCategoryPositionsTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = UpdateCategoryPositionsRequestBuilder.Build(2);
        var token = "test-user-123";

        var response = await DoPut(method, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
