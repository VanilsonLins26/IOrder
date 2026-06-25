using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public int Position { get; set; }
    public Store Store { get; set; } = new Store();
    public ICollection<Product> Products { get; set; } = [];
}
