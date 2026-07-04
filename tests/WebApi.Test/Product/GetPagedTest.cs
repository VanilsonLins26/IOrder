using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Repositories.Product;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace WebApi.Test.Product;

public class GetPagedTest : IOrderClassFixture
{
    private readonly string method = "product";
    private readonly AppDbContext _dbContext;
    public GetPagedTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task Success()
    {
        var pagedMethod = method + "/" + "paged";

        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();

        var totalProducts = await _dbContext.Products.CountAsync();
        var expectedCount = Math.Min(totalProducts, 10);
        
        responseData!.Count.ShouldBe(expectedCount);
    }

    [Fact]
    public async Task Success_With_PageSize_Limit()
    {
        var pagedMethod = $"{method}/paged?pageNumber=1&pageSize=2";
        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();
        responseData!.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Success_With_Name_Filter()
    {

        var existingProduct = _dbContext.Products.First();

        var pagedMethod = $"{method}/paged?name={existingProduct.Name}";

        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();

        responseData!.Count.ShouldBe(1);
        responseData[0].Name.ShouldBe(existingProduct.Name);
    }

    [Fact]
    public async Task Success_With_Order_By_Name()
    { 
        var expectedFirstProduct = _dbContext.Products
            .OrderBy(p => p.Name)
            .First();

        var pagedMethod = $"{method}/paged?orderBy=name";

        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();
       
        responseData.ShouldNotBeNull();
        responseData[0].Name.ShouldBe(expectedFirstProduct.Name);
    }

    [Fact]
    public async Task Success_With_Order_By_Descending_Name()
    {
        var expectedFirstProduct = _dbContext.Products
            .OrderByDescending(p => p.Name)
            .First();

        var pagedMethod = $"{method}/paged?orderBy=name&isDescending=true";

        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();

        responseData.ShouldNotBeNull();
        responseData[0].Name.ShouldBe(expectedFirstProduct.Name);
    }

    [Fact]
    public async Task Success_Filter_By_Price()
    {
        var expectedProduct = _dbContext.Products
            .First();
        var priceFilter = expectedProduct.Price.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var pagedMethod = $"{method}/paged?price={priceFilter}&priceFilter=EqualTo";

        var response = await DoGet(pagedMethod);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PagedList<ProductResponseDto>>();

        responseData!.All(p => p.Price == expectedProduct.Price).ShouldBeTrue();

    }
}
