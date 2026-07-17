using IOrder.Domain.Entities.Enums;

namespace IOrder.Domain.Entities;

public class OrderMessage : EntityBase
{
    public Guid OrderId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserRole { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime SentAt { get; init; } = DateTime.UtcNow;
    public MessageType Type { get; init; } = MessageType.Text;
    public decimal? ProposedTotalAmount { get; init; }
    public DateTime? ProposedDeliveryDate { get; init; }
    public DateTime? ReadAt { get; private set; }
    public string? ReadByUserId { get; private set; }
    public Order Order { get; private set; } = null!;

    public void MarkAsRead(string userId)
    {
        ReadAt = DateTime.UtcNow;
        ReadByUserId = userId;
    }
}
