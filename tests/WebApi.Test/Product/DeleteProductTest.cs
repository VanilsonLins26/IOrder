using CommomTestUtilities.Requests;
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

public class DeleteProductTest : IOrderClassFixture
{
    private readonly string method = "product";
    private readonly AppDbContext _dbContext;
    public DeleteProductTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var anyProduct = _dbContext.Products.FirstOrDefault();
        var deleteMethod = method + "/" + anyProduct.Id;
        var response = await DoDelete(deleteMethod);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var productExists = await _dbContext.Products.AnyAsync(product => product.Active && product.Id.Equals(anyProduct.Id));

        productExists.ShouldBeFalse();

    }

    [Fact]
    public async Task Erro_Not_Found()
    {
        var productId = Guid.CreateVersion7();
        var deleteMethod = method + "/" + productId;
        var response = await DoDelete(deleteMethod);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();

        errorResponse!.Errors.ShouldHaveSingleItem();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PRODUCT_NOT_FOUND);



    }
}
