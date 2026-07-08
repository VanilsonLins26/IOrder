using IOrder.Domain.Entities.Enums;

namespace IOrder.Domain.Entities;

public class OrderMessage : EntityBase
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public MessageType Type { get; set; } = MessageType.Text;
    public decimal? ProposedTotalAmount { get; set; }
    public DateTime? ProposedDeliveryDate { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ReadByUserId { get; set; }
    public Order Order { get; set; } = null!;
}
