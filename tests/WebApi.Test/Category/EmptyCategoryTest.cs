using IOrder.Communication.Response;
using IOrder.Exceptions;
using Shouldly;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class EmptyCategoryTest : IOrderClassFixture
{
    private readonly string method = "category";
    private readonly IOrder.Domain.Entities.Category _category;

    public EmptyCategoryTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _category = factory.CategoryList.First();
    }

    [Fact]
    public async Task Success()
    {
        var token = "test-user-123";

        var response = await DoDelete($"{method}/{_category.Id}/products", token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
