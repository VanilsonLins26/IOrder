using System;
using System.Collections.Generic;

namespace IOrder.Domain.Entities;

public class StoreCategory : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;

    public ICollection<Store> Stores { get; set; } = [];
}
