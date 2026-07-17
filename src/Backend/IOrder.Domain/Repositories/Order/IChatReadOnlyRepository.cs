namespace IOrder.Domain.Repositories.Order;

public interface IChatReadOnlyRepository
{
    Task<IList<ConversationSummary>> GetConversationsAsync(string userId, string? storeUserId, int pageNumber, int pageSize);
    Task<int> GetConversationsCountAsync(string userId, string? storeUserId);
    Task<IList<Entities.OrderMessage>> GetMessagesAsync(Guid orderId, int pageNumber, int pageSize);
    Task<int> GetMessagesCountAsync(Guid orderId);
    Task<int> GetUnreadCountAsync(Guid orderId, string userId);
}
