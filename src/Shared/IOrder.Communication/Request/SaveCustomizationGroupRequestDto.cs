using IOrder.Communication.Response;

namespace IOrder.Communication.Request;

public class SaveCustomizationGroupRequestDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "SingleChoice";
    public int MinSelections { get; set; } = 1;
    public int MaxSelections { get; set; } = 1;
    public bool Required { get; set; } = true;
    public int Position { get; set; }
    public List<SaveCustomizationOptionRequestDto> Options { get; set; } = [];
}

public class SaveCustomizationOptionRequestDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public int Position { get; set; }
}
