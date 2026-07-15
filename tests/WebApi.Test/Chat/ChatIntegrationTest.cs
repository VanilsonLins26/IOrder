using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using CommomTestUtilities.Entities;

namespace WebApi.Test.Chat;

public class ChatIntegrationTest : IOrderClassFixture
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _userToken;
    private readonly string _adminToken = "test-user-123"; // store admin

    public ChatIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _factory = factory;
        _userToken = Guid.NewGuid().ToString();
    }

    [Fact]
    public async Task GetConversations_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;

        var order = OrderBuilder.Build(store!.Id, _userToken);
        await dbContext.Orders.AddAsync(order);

        var message = new OrderMessage
        {
            UserId = order.UserId,
            UserRole = "Customer",
            Message = "Hello Store!",
            SentAt = DateTime.UtcNow
        };
        order.AddMessage(message);
        
        await dbContext.SaveChangesAsync();

        var response = await DoGet("chat/conversations", _adminToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<ConversationResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.Items.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetMessages_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;

        var order = OrderBuilder.Build(store!.Id, _userToken);
        await dbContext.Orders.AddAsync(order);

        var message = new OrderMessage
        {
            UserId = order.UserId,
            UserRole = "Customer",
            Message = "Where is my order?",
            SentAt = DateTime.UtcNow
        };
        order.AddMessage(message);
        await dbContext.SaveChangesAsync();

        var response = await DoGet($"chat/{order.Id}/messages", _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<PagedResponse<OrderMessageResponseDto>>();
        responseData.ShouldNotBeNull();
        responseData.Items.Count.ShouldBeGreaterThanOrEqualTo(1);
        responseData.Items.First().Message.ShouldBe("Where is my order?");
    }

    [Fact]
    public async Task MarkAsRead_Success()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var store = _factory.CategoryList.First().Store;

        var order = OrderBuilder.Build(store!.Id, _userToken);
        await dbContext.Orders.AddAsync(order);

        // Store sends message, user reads it
        var message = new OrderMessage
        {
            UserId = Guid.NewGuid().ToString(), // simulate store owner user id
            UserRole = "Store",
            Message = "Order confirmed!",
            SentAt = DateTime.UtcNow
        };
        order.AddMessage(message);
        await dbContext.SaveChangesAsync();

        var response = await DoPost($"chat/{order.Id}/read", null, _userToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Should return 0 unread now
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.ShouldContain("unreadCount");
        
        dbContext.ChangeTracker.Clear();
        var updatedMessage = await dbContext.OrderMessages.FindAsync(message.Id);
        updatedMessage!.ReadAt.ShouldNotBeNull();
        updatedMessage.ReadByUserId.ShouldBe(order.UserId);
    }
}
