namespace IOrder.Communication.Response;

public class SelectedOptionResponseDto
{
    public Guid OptionId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
}
