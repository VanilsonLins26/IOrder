using System.Text.Json.Serialization;

namespace IOrder.Domain.Entities;

public class CartItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid ProductId { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public string Customize { get; set; } = string.Empty;
    public List<SelectedOption> SelectedOptions { get; set; } = [];
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public Guid StoreId { get; set; }

    public decimal TotalPrice => (UnitPrice * Quantity);

    public void UpdateImageUrls(IEnumerable<string> imageUrls)
    {
        ImageUrls.Clear();
        ImageUrls.AddRange(imageUrls);
    }

}
