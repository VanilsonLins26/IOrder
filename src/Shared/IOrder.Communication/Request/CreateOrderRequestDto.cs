using IOrder.Communication.Enums;

namespace IOrder.Communication.Request;

public class CreateOrderRequestDto
{
    public string? CustomerNotes { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? CustomerPhone { get; set; }
    public DeliveryTypeDto DeliveryType { get; set; } = DeliveryTypeDto.Delivery;
    public decimal DeliveryFee { get; set; }
}
