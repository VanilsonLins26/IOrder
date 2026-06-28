using CommomTestUtilities.Requests.Product;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Product;

public class CreatePromotionTest : IOrderClassFixture
{
    private readonly string method = "product/promotion";
    private readonly AppDbContext _dbContext;
    public CreatePromotionTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var product = _dbContext.Products.First();
        var request = RequestPromotionPriceBuilder.Build();
        request.ProductId = product.Id;

        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<PromotionPriceResponseDto>();

        responseData.ShouldNotBeNull();
        responseData.Id.ShouldNotBe(Guid.Empty);
        responseData.Price.ShouldBe(request.Price!.Value);
        responseData.InitialTime.ShouldBe(request.InitialTime!.Value);
        responseData.FinalTime.ShouldBe(request.FinalTime!.Value);
        responseData.ProductId.ShouldBe(request.ProductId);

        var productExists = await _dbContext.Promotions.AnyAsync(promotion => promotion.ProductId.Equals(request.ProductId));

        productExists.ShouldBeTrue();

    }

    [Fact]
    public async Task Error_Not_Found()
    {

        var request = RequestPromotionPriceBuilder.Build();
       
        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PRODUCT_NOT_FOUND);
    }

    [Fact]
    public async Task Error_InvalidDate()
    {
        var request = RequestPromotionPriceBuilder.Build();
        request.FinalTime = request.InitialTime!.Value.AddHours(-1);

        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.FINAL_TIME_LESS_THAN_INITIAL);
    }

    [Fact]
    public async Task Error_Promotion_Price_Invalid()
    {
        var product = _dbContext.Products.First();
        var request = RequestPromotionPriceBuilder.Build();
        request.ProductId = product.Id;

        request.Price = product.Price + 10;
        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PROMOTION_PRICE_INVALID);
    }

    [Fact]
    public async Task Error_Exists_Promotion_In_Date()
    {
        var product = _dbContext.Products.First();

        var request1 = RequestPromotionPriceBuilder.Build();
        request1.ProductId = product.Id;
        await DoPost(method, request1);

        var request2 = RequestPromotionPriceBuilder.Build();
        request2.ProductId = product.Id;
        request2.InitialTime = request1.InitialTime;
        request2.FinalTime = request1.FinalTime;

        var response = await DoPost(method, request2);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.EXISTS_PROMOTION_IN_THIS_DATE);
    }
}
