using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Communication.Response;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public decimal? PromotionPrice { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool Active { get; set; } = true;
    public string? UnitOfMeasure { get; set; }
    public bool Customizable { get; set; }
    public Guid? CategoryId { get; set; }
    //public int StoreId { get; set; }
    // public StoreProductResponseDto Store { get; set; }
}
