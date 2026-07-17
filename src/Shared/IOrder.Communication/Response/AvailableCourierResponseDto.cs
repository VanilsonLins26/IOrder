namespace IOrder.Communication.Response;

public class AvailableCourierResponseDto
{
    public string CourierUserId { get; set; } = string.Empty;
    public string? CourierName { get; set; }
    public double? DistanceKm { get; set; }
    public DateTime LastLocationAt { get; set; }
}
