using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOrder.Domain.Entities;

public class Cart
{
    public string UserId { get; set; } = string.Empty;
    public string CouponCode { get; set; } = string.Empty;
    public decimal CartTotal => _itens.Sum(item => item.TotalPrice);
    [JsonInclude]
    private readonly List<CartItem> _itens = [];
    public IReadOnlyCollection<CartItem> Itens => _itens.AsReadOnly();


    public void UpdateCartItens(IEnumerable<CartItem> cartItems)
    {
        _itens.Clear();
        _itens.AddRange(cartItems);
    }
}
