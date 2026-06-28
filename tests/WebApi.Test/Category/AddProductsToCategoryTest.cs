using CommomTestUtilities.Requests.Category;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Category;

public class AddProductsToCategoryTest : IOrderClassFixture
{
    private readonly string method = "category";
    private readonly IOrder.Domain.Entities.Category _category;
    private readonly IOrder.Domain.Entities.Product _product;

    public AddProductsToCategoryTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _category = factory.CategoryList.First();
        _product = factory.ProductList.First();
    }

    [Fact]
    public async Task Success()
    {
        var request = AddProductsToCategoryRequestBuilder.Build(new List<System.Guid> { _product.Id });
        var token = "test-user-123";

        var response = await DoPost($"{method}/{_category.Id}/products", request, token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
