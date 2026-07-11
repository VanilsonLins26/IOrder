namespace IOrder.Domain.Entities;

public class SelectedOption
{
    public Guid OptionId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
}
