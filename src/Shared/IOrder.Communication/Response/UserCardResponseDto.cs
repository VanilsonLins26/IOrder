namespace IOrder.Communication.Response;

public class UserCardResponseDto
{
    public Guid Id { get; set; }
    public string LastFourDigits { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int ExpirationMonth { get; set; }
    public int ExpirationYear { get; set; }
    public string GatewayCardId { get; set; } = string.Empty;
}
