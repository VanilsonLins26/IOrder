using System.ComponentModel.DataAnnotations;

namespace IOrder.Communication.Request;

public class ProductRequestDto
{
    [Required(ErrorMessage = "Informe o nome do produto!!")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Informe o valor do produto!!")]
    [Range(0.01, 99999999999.99, ErrorMessage = "O preço deve ser maior que 0")]
    public decimal? Price { get; set; }
    [Required(ErrorMessage = "Informe a descrição!!")]
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Informe a unidade de medida!!")]
    public string? UnitOfMeasure { get; set; }
    public bool Customizable { get; set; }
    public int storeId { get; set; }
}
