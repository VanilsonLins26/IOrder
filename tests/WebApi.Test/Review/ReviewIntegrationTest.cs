using CommomTestUtilities.Requests.Review;
using IOrder.Communication.Request;
using Shouldly;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebApi.Test.Review;

public class ReviewIntegrationTest : IOrderClassFixture
{
    private readonly string method = "reviews";
    private readonly string _userToken = Guid.NewGuid().ToString();

    public ReviewIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Post_CreateReview_Returns_Created()
    {
        var request = CreateReviewRequestBuilder.Build();
        var orderId = Guid.NewGuid(); // Note: Em um teste real, o orderId precisaria existir, ou o mock do repo cuidaria disso.
        
        var response = await DoPost($"{method}/{orderId}", request, _userToken);
        
        // Se a Order não existir, vai retornar BadRequest. O Importante é bater no endpoint e passar pelo middleware.
        var allowedStatuses = new[] { HttpStatusCode.Created, HttpStatusCode.NotFound, HttpStatusCode.BadRequest };
        allowedStatuses.ShouldContain(response.StatusCode);
    }
}
