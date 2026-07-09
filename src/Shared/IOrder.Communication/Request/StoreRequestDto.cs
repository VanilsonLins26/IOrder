using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class StoreRequestDto
{
    public string? Name { get; set; }
    public AddressRequestDto Address { get; set; } = new AddressRequestDto();
    public string? About { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public ICollection<OpeningHourRequestDto> OpeningHours { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string? OwnerPhone { get; set; }
}
