namespace IOrder.Communication.Request;

public class NegotiateOrderRequestDto
{
    public decimal? ProposedTotalAmount { get; set; }
    public DateTime? ProposedDeliveryDate { get; set; }
    public string? ShopkeeperNotes { get; set; }
}
