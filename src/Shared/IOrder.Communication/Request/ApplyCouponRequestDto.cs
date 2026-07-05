using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class ApplyCouponRequestDto
{
    public string CouponCode { get; set; } = string.Empty;
}
