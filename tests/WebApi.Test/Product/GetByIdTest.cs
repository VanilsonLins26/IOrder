using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace WebApi.Test.Product;

public class GetByIdTest : IOrderClassFixture
{
    private readonly string method = "product";
    private readonly AppDbContext _dbContext;
    public GetByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var anyProduct = _dbContext.Products.FirstOrDefault();
        var methodById = method + "/" + anyProduct.Id;
        var response = await DoGet(methodById);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Id.ShouldBe(anyProduct.Id);
        responseData.Name.ShouldBe(anyProduct.Name);
        responseData.Price.ShouldBe(anyProduct.Price);
        responseData.ImageUrl.ShouldBe(anyProduct.ImageUrl);
        responseData.Description.ShouldBe(anyProduct.Description);
        responseData.Customizable.ShouldBe(anyProduct.Customizable);

    }

    [Fact]
    public async Task Erro_Not_Found()
    {
        var productId = Guid.CreateVersion7();
        var methodById = method + "/" + productId;
        var response = await DoGet(methodById);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PRODUCT_NOT_FOUND);



    }
}
