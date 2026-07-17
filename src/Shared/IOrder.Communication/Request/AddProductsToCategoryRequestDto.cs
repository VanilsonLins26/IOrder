using System;
using System.Collections.Generic;

namespace IOrder.Communication.Request;

public class AddProductsToCategoryRequestDto
{
    public IList<Guid> ProductIds { get; set; } = [];
}
