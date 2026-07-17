using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Request;

public class UpdateStoreRequestDto
{
    public string? Name { get; set; }
    public string? About { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? BaseDeliveryFee { get; set; }
    public decimal? FeePerKm { get; set; }
    public double? MaxDeliveryDistanceKm { get; set; }
    public double? FreeDeliveryRadiusKm { get; set; }
    public int? DeliveryPartner { get; set; }
}
