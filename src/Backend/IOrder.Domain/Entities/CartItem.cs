using System.Text.Json.Serialization;

namespace IOrder.Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPrice { get { return (UnitPrice * Quantity); } }
    [JsonInclude]
    private readonly List<string> _imageUrls = [];
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();
    public string Customize { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;


    public void UpdateImageUrls(IEnumerable<string> imageUrls)
    {
        _imageUrls.Clear();
        _imageUrls.AddRange(imageUrls);
    }

}
