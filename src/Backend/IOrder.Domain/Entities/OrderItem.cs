namespace IOrder.Domain.Entities;

public class OrderItem : EntityBase
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Customize { get; set; } = string.Empty;
    public Order Order { get; set; } = null!;

    public decimal TotalPrice => UnitPrice * Quantity;
}
