using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class CategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int? Position { get; set; }
}
