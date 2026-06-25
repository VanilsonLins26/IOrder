using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class UpdateStoreRequestDto
{
    public string? Name { get; set; }
    public string? About { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
