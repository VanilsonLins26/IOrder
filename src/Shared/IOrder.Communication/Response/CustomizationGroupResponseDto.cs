namespace IOrder.Communication.Response;

public class CustomizationGroupResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int MinSelections { get; set; }
    public int MaxSelections { get; set; }
    public bool Required { get; set; }
    public int Position { get; set; }
    public List<CustomizationOptionResponseDto> Options { get; set; } = [];
}

public class CustomizationOptionResponseDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public int Position { get; set; }
}
