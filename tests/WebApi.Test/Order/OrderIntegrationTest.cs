using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Requests.Order;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Order;

public class OrderIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _userToken = Guid.NewGuid().ToString();
    private readonly string _adminToken = "test-user-123";

    public OrderIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrder_Success()
    {
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        await DoPatch("Cart/AddItem", addRequest, _userToken);

        var request = CreateOrderRequestBuilder.Build();

        var response = await DoPost("order", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.CustomerNotes.ShouldBe(request.CustomerNotes);
        responseData.Items.ShouldNotBeEmpty();
        responseData.Items.First().ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public async Task CreateOrder_Error_EmptyCart()
    {
        var request = CreateOrderRequestBuilder.Build();

        var response = await DoPost("order", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse.ShouldNotBeNull();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.INVALID_CART);
    }

    [Fact]
    public async Task GetOrderById_Success()
    {
        var orderId = await SeedOrderForUser(_userToken);

        var response = await DoGet($"order/{orderId}", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Id.ShouldBe(orderId);
    }

    [Fact]
    public async Task GetOrderById_Error_NotFound()
    {
        var response = await DoGet($"order/{Guid.NewGuid()}", _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserOrders_Success()
    {
        var orderId = await SeedOrderForUser(_userToken);

        var response = await DoGet("order/user?pageNumber=1&pageSize=10", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<OrderResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.Items.ShouldContain(o => o.Id == orderId);
    }

    [Fact]
    public async Task GetStoreOrders_Success()
    {
        var orderId = await SeedOrderForUser(Guid.NewGuid().ToString());

        var response = await DoGet("order/store?pageNumber=1&pageSize=10", _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<OrderResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.Items.ShouldContain(o => o.Id == orderId);
    }

    [Fact]
    public async Task UpdateOrderStatus_Success()
    {
        var orderId = await SeedOrderForUser(Guid.NewGuid().ToString());

        var request = new UpdateOrderStatusRequestDto
        {
            Status = OrderStatusDto.AwaitingPayment
        };

        var response = await DoPatch($"order/{orderId}/status", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Status.ShouldBe(OrderStatusDto.AwaitingPayment);
    }

    [Fact]
    public async Task UpdateOrderStatus_Error_Unauthorized()
    {
        var orderId = await SeedOrderForUser(_userToken);

        var request = new UpdateOrderStatusRequestDto
        {
            Status = OrderStatusDto.Preparing
        };

        var response = await DoPatch($"order/{orderId}/status", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NegotiateOrder_Success()
    {
        var orderId = await SeedOrderForUser(Guid.NewGuid().ToString());

        var request = NegotiateOrderRequestBuilder.Build();

        var response = await DoPatch($"order/{orderId}/negotiate", request, _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Status.ShouldBe(OrderStatusDto.Negotiating);
    }

    [Fact]
    public async Task SendMessage_Success()
    {
        var orderId = await SeedOrderForUser(_userToken);

        var request = SendOrderMessageRequestBuilder.Build();

        var response = await DoPost($"order/{orderId}/message", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Messages.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SendMessage_Error_Empty()
    {
        var orderId = await SeedOrderForUser(_userToken);

        var request = SendOrderMessageRequestBuilder.Build();
        request.Message = string.Empty;

        var response = await DoPost($"order/{orderId}/message", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> SeedOrderForUser(string userToken)
    {
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        await DoPatch("Cart/AddItem", addRequest, userToken);

        var createRequest = CreateOrderRequestBuilder.Build();
        var createResponse = await DoPost("order", createRequest, userToken);
        var order = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();
        return order!.Id;
    }
}
