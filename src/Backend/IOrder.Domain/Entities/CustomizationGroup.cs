using IOrder.Domain.Entities.Enums;

namespace IOrder.Domain.Entities;

public class CustomizationGroup : EntityBase
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CustomizationGroupType Type { get; set; }
    public int MinSelections { get; set; } = 1;
    public int MaxSelections { get; set; } = 1;
    public bool Required { get; set; } = true;
    public int Position { get; set; }
    public Product Product { get; set; } = null!;
    private readonly List<CustomizationOption> _options = [];
    public IReadOnlyCollection<CustomizationOption> Options => _options.AsReadOnly();

    public void AddOption(CustomizationOption option)
    {
        _options.Add(option);
    }
}
