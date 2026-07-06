using CommomTestUtilities.Entities;
using CommomTestUtilities.Requests.Coupon;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Coupon;

public class CouponIntegrationTest : IOrderClassFixture
{
    private readonly AppDbContext _dbContext;
    private readonly string _adminToken = "test-user-123";

    public CouponIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task CreateCoupon_Success()
    {
        var request = CouponRequestBuilder.Build();

        var response = await DoPost("coupon", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<CouponResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Code.ShouldBe(request.Code);
        responseData.DiscountType.ShouldBe(request.DiscountType);
        responseData.DiscountValue.ShouldBe(request.DiscountValue);
    }

    [Fact]
    public async Task CreateCoupon_Error_Code_Already_Exists()
    {
        var existingCoupon = CouponBuilder.Build("UNIQUECODE");
        _dbContext.Coupons.Add(existingCoupon);
        await _dbContext.SaveChangesAsync();

        var request = CouponRequestBuilder.Build();
        request.Code = "UNIQUECODE";

        var response = await DoPost("coupon", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse.ShouldNotBeNull();
        errorResponse.Errors.ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.COUPON_CODE_EXISTS);
    }

    [Fact]
    public async Task CreateCoupon_Error_Validation_Failed()
    {
        var request = CouponRequestBuilder.Build();
        request.Code = string.Empty;

        var response = await DoPost("coupon", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse.ShouldNotBeNull();
        errorResponse.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetActiveCoupons_Success()
    {
        var coupon = CouponBuilder.Build("ACTIVE10");
        _dbContext.Coupons.Add(coupon);
        await _dbContext.SaveChangesAsync();

        var response = await DoGet("coupon/active");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<IList<CouponResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.ShouldContain(c => c.Code == "ACTIVE10");
    }

    [Fact]
    public async Task GetAllCoupons_Success()
    {
        var coupon1 = CouponBuilder.Build("ALL01");
        var coupon2 = CouponBuilder.Build("ALL02");
        _dbContext.Coupons.AddRange(coupon1, coupon2);
        await _dbContext.SaveChangesAsync();

        var response = await DoGet("coupon/admin", _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<IList<CouponResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.ShouldContain(c => c.Code == "ALL01");
        responseData.ShouldContain(c => c.Code == "ALL02");
    }
}
