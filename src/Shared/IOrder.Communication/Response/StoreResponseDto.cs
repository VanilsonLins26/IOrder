using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IOrder.Communication.Response;

public class StoreResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public AddressResponseDto? Address { get; set; }
    public string? About { get; set; }
    public string ImageUrl { get; set; }
    public ICollection<ProductResponseDto> Products { get; set; } = [];
    public ICollection<OpeningHourResponseDto> OpeningHours { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public String UserId { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
}
