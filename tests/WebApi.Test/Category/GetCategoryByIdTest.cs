using IOrder.Communication.Response;
using IOrder.Exceptions;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class GetCategoryByIdTest : IOrderClassFixture
{
    private readonly string method = "category";
    private readonly IOrder.Domain.Entities.Category _category;

    public GetCategoryByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _category = factory.CategoryList.First();
    }

    [Fact]
    public async Task Success()
    {
        var token = "test-user-123";

        var response = await DoGet($"{method}/{_category.Id}", token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        responseData.ShouldNotBeNull();
        responseData!.Name.ShouldBe(_category.Name);
    }

    [Fact]
    public async Task Error_Category_Not_Found()
    {
        var token = "test-user-123";

        var response = await DoGet($"{method}/{System.Guid.NewGuid()}", token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.CATEGORY_NOT_FOUND);
    }
}
