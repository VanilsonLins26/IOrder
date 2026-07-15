using System;

namespace IOrder.Communication.Request;

public class StoreSearchRequestDto : PaginationRequestDto
{
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
    public double? UserLatitude { get; set; }
    public double? UserLongitude { get; set; }
}
