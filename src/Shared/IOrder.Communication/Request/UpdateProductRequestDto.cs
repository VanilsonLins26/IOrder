using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IOrder.Communication.Request;

public class UpdateProductRequestDto
{
    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool Customizable { get; set; }
    public Guid? CategoryId { get; set; }
    //public int storeId { get; set; }
}