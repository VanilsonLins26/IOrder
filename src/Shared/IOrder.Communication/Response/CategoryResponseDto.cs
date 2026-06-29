using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Response;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public int Position { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public ICollection<ProductResponseDto> Products { get; set; } = [];
}
