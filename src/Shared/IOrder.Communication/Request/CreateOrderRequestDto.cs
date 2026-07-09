namespace IOrder.Communication.Request;

public class CreateOrderRequestDto
{
    public string? CustomerNotes { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? CustomerPhone { get; set; }
}
