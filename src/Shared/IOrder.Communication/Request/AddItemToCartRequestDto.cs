using System.Text.Json.Serialization;

namespace IOrder.Communication.Request;

public class AddItemToCartRequestDto
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public string Customize { get; set; } = string.Empty;
}
