using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Domain.Entities;

public class Store
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.Now;
    public string? About { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<OpeningHour> OpeningHours { get; set; } = [];
    public int CategoryId { get; set; }
    public String UserId { get; set; }
    //public Category? Category { get; set; }

}
