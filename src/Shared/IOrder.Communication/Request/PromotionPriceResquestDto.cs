using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class PromotionPriceResquestDto
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime InitialTime { get; set; }
    public DateTime FinalTime { get; set; }
}
