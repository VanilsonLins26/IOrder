using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Response;

public class CategoryResponseDto
{
    public string Name { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public int Position { get; set; }
}
