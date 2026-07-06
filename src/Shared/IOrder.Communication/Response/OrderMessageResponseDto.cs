using IOrder.Communication.Enums;

namespace IOrder.Communication.Response;

public class OrderMessageResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public MessageTypeDto Type { get; set; }
    public decimal? ProposedTotalAmount { get; set; }
    public DateTime? ProposedDeliveryDate { get; set; }
}
