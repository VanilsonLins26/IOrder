namespace IOrder.Communication.Response;

public class PaymentIntentResponseDto
{
    public string ClientSecret { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}
