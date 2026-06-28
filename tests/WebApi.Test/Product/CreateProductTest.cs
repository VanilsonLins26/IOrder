using CommomTestUtilities.Requests.Product;
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
using System.Text.Json;

namespace WebApi.Test.Product;

public class CreateProductTest : IOrderClassFixture
{
    private readonly string method = "product";
    private readonly AppDbContext _dbContext;
    public CreateProductTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestCreateProductBuilder.Build();

        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);


        var responseData = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

        responseData.ShouldNotBeNull();
        responseData.Id.ShouldNotBe(Guid.Empty);
        responseData.Name.ShouldBe(request.Name);
        responseData.Price.ShouldBe(request.Price!.Value);
        responseData.Description.ShouldBe(request.Description);
        responseData.ImageUrl.ShouldBe(request.ImageUrl);
        responseData.Customizable.ShouldBe(request.Customizable);
        responseData.Active.ShouldBeTrue();

        var productExists = await _dbContext.Products.AnyAsync(product => product.Active && product.Name.Equals(request.Name));

        productExists.ShouldBeTrue();

    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestCreateProductBuilder.Build();
        request.Name = string.Empty;

        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);


        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.NAME_EMPTY);

        var productExists = await _dbContext.Products.AnyAsync(product => product.Active && product.Name.Equals(request.Name));

        productExists.ShouldBeFalse();
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var existingProduct = _dbContext.Products.First();

        var request = RequestCreateProductBuilder.Build();
        request.Name = existingProduct.Name;


        var response = await DoPost(method, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();

        errorResponse.Errors.ShouldContain(ResourceMessagesException.NAME_ALREADY_EXISTS);
    }
}
