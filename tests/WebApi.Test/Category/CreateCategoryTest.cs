using CommomTestUtilities.Requests.Category;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class CreateCategoryTest : IOrderClassFixture
{
    private readonly string method = "category";

    public CreateCategoryTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = CategoryRequestBuilder.Build();
        var token = "test-user-123";

        var response = await DoPost(method, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
        responseData.ShouldNotBeNull();
        responseData!.Name.ShouldBe(request.Name);
    }

    [Fact]
    public async Task Error_Without_Store()
    {
        var request = CategoryRequestBuilder.Build();
        var token = "test-new-user";

        var response = await DoPost(method, request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldContain(ResourceMessagesException.STORE_NOT_FOUND);
    }
}
