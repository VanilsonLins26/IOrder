using System;
using IOrder.Communication.Enums;

namespace IOrder.Communication.Request;

public class ProductSearchRequestDto : PaginationRequestDto
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public PriceFilterTypeDto? PriceFilter { get; set; }
    public Guid? StoreId { get; set; }
}
