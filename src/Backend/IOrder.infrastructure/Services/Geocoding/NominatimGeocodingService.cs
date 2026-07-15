using System.Net.Http.Json;
using IOrder.Domain.Services;

namespace IOrder.infrastructure.Services.Geocoding;

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public NominatimGeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string street, string city, string state, string zipCode)
    {
        try
        {
            // ZipCode pode ajudar mas as vezes o Nominatim falha no Brasil com CEP. 
            // Vamos usar street, city, state.
            var requestUri = $"https://nominatim.openstreetmap.org/search?street={Uri.EscapeDataString(street)}&city={Uri.EscapeDataString(city)}&state={Uri.EscapeDataString(state)}&country=Brazil&format=json";
            
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("User-Agent", "IOrderApp/1.0 (contact@iorder.com)");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var results = await response.Content.ReadFromJsonAsync<List<NominatimResponse>>();
            if (results != null && results.Count > 0)
            {
                var first = results[0];
                if (double.TryParse(first.Lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(first.Lon, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lon))
                {
                    return (lat, lon);
                }
            }
        }
        catch
        {
            // Ignore failure, just return null
        }
        
        return null;
    }
    
    private class NominatimResponse
    {
        public string Lat { get; set; } = string.Empty;
        public string Lon { get; set; } = string.Empty;
    }
}
