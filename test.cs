using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

public class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid ProductId { get; set; }
    public decimal TotalPrice { get { return (UnitPrice * Quantity); } }
    public string Customize { get; set; } = string.Empty;
}

public class Cart
{
    public string UserId { get; set; } = string.Empty;
    public string CouponCode { get; set; } = string.Empty;
    public decimal CartTotal => _itens.Sum(item => item.TotalPrice);
    [JsonInclude]
    [JsonPropertyName("Itens")] // test if this helps
    private readonly List<CartItem> _itens = new List<CartItem>();
    
    [JsonIgnore]
    public IReadOnlyCollection<CartItem> Itens => _itens.AsReadOnly();

    public void AddCartItem(CartItem cartItem)
    {
        _itens.Add(cartItem);
    }
}

class Program
{
    static void Main()
    {
        var cart = new Cart { UserId = "123" };
        cart.AddCartItem(new CartItem { Quantity = 2, UnitPrice = 10 });
        
        var json = JsonSerializer.Serialize(cart);
        Console.WriteLine("Serialized: " + json);
        
        var deserialized = JsonSerializer.Deserialize<Cart>(json);
        Console.WriteLine("Deserialized Itens Count: " + deserialized.Itens.Count);
    }
}
