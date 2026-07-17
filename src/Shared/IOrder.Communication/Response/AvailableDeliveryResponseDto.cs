namespace IOrder.Communication.Response;

public class AvailableDeliveryResponseDto
{
    public Guid OrderId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreImageUrl { get; set; } = string.Empty;
    public string StoreAddress { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public double DistanceKm { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public DateTime? DeliveryDateEnd { get; set; }
    public bool RequestedEarlyDelivery { get; set; }
    public DateTime CreatedAt { get; set; }
}
