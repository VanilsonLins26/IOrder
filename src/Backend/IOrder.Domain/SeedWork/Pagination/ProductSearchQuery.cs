using IOrder.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.SeedWork.Pagination;

public class ProductSearchQuery : PaginationQuery
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public PriceFilterType? PriceFilter { get; set; }
    public Guid? StoreId { get; set; }
}
