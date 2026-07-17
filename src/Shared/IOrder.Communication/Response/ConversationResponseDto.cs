namespace IOrder.Communication.Response;

public class ConversationResponseDto
{
    public Guid OrderId { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreImageUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessageByRole { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
