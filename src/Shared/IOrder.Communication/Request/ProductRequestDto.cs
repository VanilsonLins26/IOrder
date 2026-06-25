using IOrder.Communication.Enums;
using System.ComponentModel.DataAnnotations;

namespace IOrder.Communication.Request;

public class ProductRequestDto
{

    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }
    public bool Customizable { get; set; }
}
