namespace IOrder.Domain.Entities;

public class CustomizationOption : EntityBase
{
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public int Position { get; set; }
    public CustomizationGroup Group { get; set; } = null!;
}
