namespace IOrder.Communication.Response;

public class CourierLocationResponseDto
{
    public string CourierUserId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
}
