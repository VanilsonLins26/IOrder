using System;
using System.Collections.Generic;
using System.Text;
using IOrder.Domain.SeedWork;

namespace IOrder.Domain.Entities;

public class Category : EntityBase, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public int Position { get; set; }
    public Store Store { get; set; } = default!;
}
