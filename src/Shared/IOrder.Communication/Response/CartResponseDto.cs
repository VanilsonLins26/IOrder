using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Response;

public class CartResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public decimal CartTotal { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = [];
}