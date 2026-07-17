using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class ChangeCartItemQuantityRequestDto
{
    public Guid CartItemId { get; set; }
    public int NewQuantity { get; set; }
}
