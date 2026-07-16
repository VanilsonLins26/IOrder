using NetTopologySuite.Geometries;

namespace IOrder.Domain.Entities;

public class CourierLocation
{
    public string CourierUserId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Point? Location { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
