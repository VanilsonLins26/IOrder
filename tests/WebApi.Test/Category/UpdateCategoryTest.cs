using CommomTestUtilities.Requests.Category;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class UpdateCategoryTest : IOrderClassFixture
{
    private readonly string method = "category";
    private readonly IOrder.Domain.Entities.Category _category;

    public UpdateCategoryTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _category = factory.CategoryList.First();
    }

    [Fact]
    public async Task Success()
    {
        var request = CategoryRequestBuilder.Build();
        var token = "test-user-123";

        var response = await DoPut($"{method}/{_category.Id}", request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        responseData.ShouldNotBeNull();
        responseData!.Name.ShouldBe(request.Name);
    }

    [Fact]
    public async Task Error_Category_Not_Found()
    {
        var request = CategoryRequestBuilder.Build();
        var token = "test-user-123";

        var response = await DoPut($"{method}/{System.Guid.NewGuid()}", request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.CATEGORY_NOT_FOUND);
    }
}
