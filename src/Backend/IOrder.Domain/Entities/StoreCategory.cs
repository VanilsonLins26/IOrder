using System;
using System.Collections.Generic;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class StoreCategory : EntityBase, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
}
