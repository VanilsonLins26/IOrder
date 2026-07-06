using IOrder.Communication.Request;
using IOrder.Communication.Response;
using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

using CommomTestUtilities.Requests.Cart;

namespace WebApi.Test.Cart;

public class CartIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _token = "test-token"; // Using TestAuthHandler which ignores token validation

    public CartIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddItemToCart_Success()
    {
        var product = _factory.ProductList.First();

        var request = AddItemToCartRequestBuilder.Build();
        request.ProductId = product.Id;

        var response = await DoPatch("Cart/AddItem", request, _token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var cartResponse = await response.Content.ReadFromJsonAsync<CartResponseDto>();
        cartResponse.ShouldNotBeNull();
        cartResponse.Items.ShouldContain(i => i.ProductId == product.Id);
    }

    [Fact]
    public async Task GetCart_Success()
    {
        // First add an item to ensure cart exists
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        
        await DoPatch("Cart/AddItem", addRequest, _token);

        // Act
        var response = await DoGet("Cart", _token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cartResponse = await response.Content.ReadFromJsonAsync<CartResponseDto>();
        cartResponse.ShouldNotBeNull();
    }

    [Fact]
    public async Task ChangeItemQuantity_Success()
    {
        // Add an item first
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        
        var addResponse = await DoPatch("Cart/AddItem", addRequest, _token);
        var cart = await addResponse.Content.ReadFromJsonAsync<CartResponseDto>();
        var cartItem = cart!.Items.First(i => i.ProductId == product.Id);

        // Act - Change Quantity
        var changeRequest = ChangeCartItemQuantityRequestBuilder.Build();
        changeRequest.CartItemId = cartItem.Id;
        
        var response = await DoPatch("Cart/ChangeQuantity", changeRequest, _token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cartResponse = await response.Content.ReadFromJsonAsync<CartResponseDto>();
        cartResponse.ShouldNotBeNull();
        var updatedItem = cartResponse.Items.First(i => i.Id == cartItem.Id);
        updatedItem.Quantity.ShouldBe(changeRequest.NewQuantity);
    }

    [Fact]
    public async Task AddCoupon_Success()
    {
        // Add an item first so cart exists
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        await DoPatch("Cart/AddItem", addRequest, _token);

        var request = ApplyCouponRequestBuilder.Build();

        var response = await DoPatch("Cart/Coupon", request, _token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cartResponse = await response.Content.ReadFromJsonAsync<CartResponseDto>();
        cartResponse.ShouldNotBeNull();
        cartResponse.CouponCode.ShouldBe(request.CouponCode);
    }

    [Fact]
    public async Task RemoveItemFromCart_Success()
    {
        // Add an item first
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        var addResponse = await DoPatch("Cart/AddItem", addRequest, _token);
        var cart = await addResponse.Content.ReadFromJsonAsync<CartResponseDto>();
        var cartItem = cart!.Items.First(i => i.ProductId == product.Id);

        // Act - Remove Item
        var response = await DoDelete($"Cart/{cartItem.Id}", _token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify it was removed
        var getResponse = await DoGet("Cart", _token);
        var updatedCart = await getResponse.Content.ReadFromJsonAsync<CartResponseDto>();
        updatedCart.ShouldNotBeNull();
        updatedCart.Items.ShouldNotContain(i => i.Id == cartItem.Id);
    }

    [Fact]
    public async Task ClearCart_Success()
    {
        // Add items first
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        await DoPatch("Cart/AddItem", addRequest, _token);

        // Act - Clear Cart
        var response = await DoDelete("Cart", _token);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify cart is empty
        var getResponse = await DoGet("Cart", _token);
        var updatedCart = await getResponse.Content.ReadFromJsonAsync<CartResponseDto>();
        updatedCart.ShouldNotBeNull();
        updatedCart.Items.ShouldBeEmpty();
    }
}
