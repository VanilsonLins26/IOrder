using CommomTestUtilities.Entities;
using CommomTestUtilities.Requests.Cart;
using CommomTestUtilities.Requests.Payment;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.Exceptions;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Payment;

public class PaymentIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _userToken;

    public PaymentIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _userToken = Guid.NewGuid().ToString();
    }

    [Fact]
    public async Task CreatePayment_Success()
    {
        var orderId = await SeedAcceptedOrder();

        var request = CreatePaymentRequestBuilder.BuildPix();
        request.OrderId = orderId;
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.OrderId.ShouldBe(orderId);
        responseData.Method.ShouldBe(PaymentMethodDto.Pix);
        responseData.PixQrCode.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreatePayment_Success_CreditCard()
    {
        var orderId = await SeedAcceptedOrder();

        var request = CreatePaymentRequestBuilder.BuildCreditCard();
        request.OrderId = orderId;
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.OrderId.ShouldBe(orderId);
        responseData.Method.ShouldBe(PaymentMethodDto.CreditCard);
        responseData.CardLastFourDigits.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreatePayment_Success_Boleto()
    {
        var orderId = await SeedAcceptedOrder();

        var request = CreatePaymentRequestBuilder.BuildBoleto();
        request.OrderId = orderId;
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var responseData = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.OrderId.ShouldBe(orderId);
        responseData.Method.ShouldBe(PaymentMethodDto.Boleto);
        responseData.BoletoUrl.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreatePayment_Error_Order_Not_Awaiting()
    {
        var orderId = await SeedOrder();

        var request = CreatePaymentRequestBuilder.BuildPix();
        request.OrderId = orderId;
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var errorResponse = await response.Content.ReadFromJsonAsync<ResponseErrorDto>();
        errorResponse.ShouldNotBeNull();
        errorResponse.Errors.ShouldContain(ResourceMessagesException.PAYMENT_ORDER_NOT_AWAITING);
    }

    [Fact]
    public async Task CreatePayment_Error_Order_Not_Found()
    {
        var request = CreatePaymentRequestBuilder.BuildPix();
        request.OrderId = Guid.NewGuid();
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePayment_Error_Validation()
    {
        var request = CreatePaymentRequestBuilder.Build();
        request.PayerEmail = string.Empty;
        request.OrderId = Guid.NewGuid();
        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPaymentByOrder_Success()
    {
        var orderId = await SeedAcceptedOrder();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var payment = PaymentBuilder.BuildPix(orderId);
        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        var response = await DoGet($"payment/{orderId}", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseData = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.OrderId.ShouldBe(orderId);
    }

    [Fact]
    public async Task GetPaymentByOrder_Error_Not_Found()
    {
        var response = await DoGet($"payment/{Guid.NewGuid()}", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Webhook_Success()
    {
        var payload = "{\"action\":\"payment.created\",\"data\":{\"id\":\"12345\"}}";
        var response = await DoPost("payment/webhook", payload, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private async Task<Guid> SeedOrder()
    {
        var product = _factory.ProductList.First();
        var addRequest = AddItemToCartRequestBuilder.Build();
        addRequest.ProductId = product.Id;
        await DoPatch("Cart/AddItem", addRequest, _userToken);

        var createRequest = new CreateOrderRequestDto
        {
            CustomerNotes = "Test order",
            DeliveryDate = DateTime.UtcNow.AddDays(1),
            CustomerPhone = "11999999999"
        };
        var createResponse = await DoPost("order", createRequest, _userToken);
        var order = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();
        return order!.Id;
    }

    private async Task<Guid> SeedAcceptedOrder()
    {
        var orderId = await SeedOrder();

        var updateRequest = new UpdateOrderStatusRequestDto
        {
            Status = OrderStatusDto.AwaitingPayment
        };
        await DoPatch($"order/{orderId}/status", updateRequest, "test-user-123");

        return orderId;
    }
}
