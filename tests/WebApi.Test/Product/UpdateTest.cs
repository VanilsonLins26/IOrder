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

namespace WebApi.Test.Product;

public class UpdateTest : IOrderClassFixture
{
    private readonly string method = "product";
    private readonly AppDbContext _dbContext;
    public UpdateTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var firstProduct = _dbContext.Products.FirstOrDefault();
        var request = RequestUpdateProductBuilder.Build();
        var updateMethod = $"{method}/{firstProduct!.Id}";

        var response = await DoPut(updateMethod, request);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);


        var responseData = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

        responseData.ShouldNotBeNull();
        responseData.Id.ShouldBe(firstProduct.Id);
        responseData.Name.ShouldBe(request.Name);
        responseData.Price.ShouldBe(request.Price!.Value);
        responseData.Description.ShouldBe(request.Description);
        responseData.ImageUrl.ShouldBe(request.ImageUrl);
        responseData.Active.ShouldBeTrue();

        var productExists = await _dbContext.Products.AnyAsync(product => product.Active && product.Id.Equals(firstProduct.Id));

        productExists.ShouldBeTrue();

    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var existingProduct = _dbContext.Products.First(); ;
        var request = RequestUpdateProductBuilder.Build();
        var updateMethod = $"{method}/{existingProduct.Id}";
        request.Name = string.Empty;
        var response = await DoPut(updateMethod, request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);


        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.NAME_EMPTY);
    }

    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var existingProducts = _dbContext.Products.Take(2).ToList();

        var request = RequestUpdateProductBuilder.Build();
        var updateMethod = $"{method}/{existingProducts[0].Id}";

        request.Name = existingProducts[1].Name;
        var response = await DoPut(updateMethod, request);
        

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();

        errorResponse.Errors.ShouldContain(ResourceMessagesException.NAME_ALREADY_EXISTS);
    }

    [Fact]
    public async Task Error_Not_Found()
    {
        var productId = Guid.CreateVersion7();
        var updateMethod = $"{method}/{productId}";

        var request = RequestUpdateProductBuilder.Build();

        var response = await DoPut(updateMethod, request);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PRODUCT_NOT_FOUND);
    }
}
