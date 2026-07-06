using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOrder.Communication.Response;

public class CartItemResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPrice { get { return (UnitPrice * Quantity); } }
    public List<string> ImageUrls { get; set; } = [];
    public string Customize { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
}
