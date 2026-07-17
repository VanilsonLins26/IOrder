using IOrder.Communication.Enums;

namespace IOrder.Communication.Response;

public class DeliveryAssignmentResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string CourierUserId { get; set; } = string.Empty;
    public AssignmentStatusDto Status { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? InTransitAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? CourierNotes { get; set; }
}
