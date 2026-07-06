namespace IOrder.Communication.Request;

public class CreateOrderRequestDto
{
    public string? CustomerNotes { get; set; }
    public DateTime? DeliveryDate { get; set; }
}
