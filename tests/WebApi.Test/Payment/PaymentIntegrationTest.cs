using CommomTestUtilities.Entities;
using CommomTestUtilities.Requests.Payment;
using IOrder.Communication.Enums;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities.Enums;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace WebApi.Test.Payment;

public class PaymentIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _userToken = Guid.NewGuid().ToString();

    public PaymentIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePayment_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;
        var order = OrderBuilder.Build(store!.Id, _userToken);
        order.Accept(); // Sets status to AwaitingPayment
        
        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        var request = CreatePaymentRequestBuilder.Build(order.Id);

        var paymentResponse = new PaymentIntentResponseDto
        {
            PaymentIntentId = "pi_mock",
            ClientSecret = "secret_mock",
            CustomerId = "cus_mock"
        };

        _factory.PaymentMock.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<string?>()))
            .ReturnsAsync(paymentResponse);

        _factory.PaymentMock.Setup(x => x.GetOrCreateCustomerAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("cus_mock");

        var response = await DoPost("payment", request, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var responseData = await response.Content.ReadFromJsonAsync<PaymentIntentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.ClientSecret.ShouldNotBeNullOrWhiteSpace();
        responseData.PaymentIntentId.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetPaymentByOrder_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;
        var order = OrderBuilder.Build(store!.Id, _userToken);
        await dbContext.Orders.AddAsync(order);

        var payment = PaymentBuilder.Build(order.Id);
        // Arrendondar para 2 casas decimais porque MySQL grava com 2 casas
        payment.Amount = Math.Round(payment.Amount, 2);

        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        var response = await DoGet($"payment/{order.Id}", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PaymentResponseDto>();
        responseData.ShouldNotBeNull();
        responseData.Id.ShouldBe(payment.Id);
        responseData.OrderId.ShouldBe(payment.OrderId);
        responseData.Amount.ShouldBe(Math.Round(payment.Amount, 2));
    }

    [Fact]
    public async Task ProcessWebhook_Success()
    {
        // For the webhook, we are using a mock IPaymentService in CustomWebApplicationFactory
        // We will seed an order and a payment, then call the webhook.

        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;
        var order = OrderBuilder.Build(store!.Id, _userToken);
        await dbContext.Orders.AddAsync(order);

        var payment = PaymentBuilder.Build(order.Id);
        await dbContext.Payments.AddAsync(payment);
        await dbContext.SaveChangesAsync();

        var paymentResponse = new PaymentResponseDto
        {
            Id = payment.Id,
            Status = PaymentStatusDto.Approved,
            OrderId = payment.OrderId,
            Amount = payment.Amount
        };

        _factory.PaymentMock.Setup(s => s.ProcessWebhookAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(paymentResponse);

        // Webhook usually doesn't have a token, but the route might not have Authorize attribute, let's just post payload
        var response = await DoPost("payment/webhook", new { data = "test" }, "");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
