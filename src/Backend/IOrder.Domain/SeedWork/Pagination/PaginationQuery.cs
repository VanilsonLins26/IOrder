using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.SeedWork.Pagination;

public class PaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsDescending { get; set; } = false;


}