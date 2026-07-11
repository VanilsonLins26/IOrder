using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using IOrder.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOrder.infrastructure.Services.Evolution;

[ExcludeFromCodeCoverage]
public class EvolutionApiService : IEvolutionApiService
{
    private readonly ILogger<EvolutionApiService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _instanceName;
    private readonly string _apiKey;

    public EvolutionApiService(IConfiguration configuration, ILogger<EvolutionApiService> logger)
    {
        _logger = logger;
        _instanceName = configuration["EvolutionApi:InstanceName"] ?? "";
        _apiKey = configuration["EvolutionApi:ApiKey"] ?? "";
        var baseUrl = configuration["EvolutionApi:BaseUrl"] ?? "";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
        };
        _httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
    }

    public async Task SendTextAsync(string phoneNumber, string message)
    {
        try
        {
            var normalized = NormalizePhone(phoneNumber);
            var payload = new SendTextRequest
            {
                Number = normalized,
                Text = message
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"message/sendText/{_instanceName}", payload);

            if (response.IsSuccessStatusCode)
            {
                    _logger.LogInformation(
                        "WhatsApp sent to {PhoneNumber} (normalized: {Normalized})", phoneNumber, normalized);
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "WhatsApp API returned {StatusCode} for {PhoneNumber}: {Body}",
                    (int)response.StatusCode, phoneNumber, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp to {PhoneNumber}", phoneNumber);
        }
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (!digits.StartsWith("55"))
            digits = "55" + digits;
        return digits;
    }

    private class SendTextRequest
    {
        [JsonPropertyName("number")]
        public string Number { get; set; } = "";

        [JsonPropertyName("text")]
        public string Text { get; set; } = "";
    }
}
