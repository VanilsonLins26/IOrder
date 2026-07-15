namespace IOrder.Domain.Services;

public interface IGeocodingService
{
    Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string street, string city, string state, string zipCode);
}
