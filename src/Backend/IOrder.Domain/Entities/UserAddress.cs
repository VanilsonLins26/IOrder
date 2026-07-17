using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class UserAddress : EntityBase
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // e.g. Home, Work
    public string ZipCode { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Complement { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public NetTopologySuite.Geometries.Point? Location { get; set; }
    
    public bool IsDefault { get; set; }
}
