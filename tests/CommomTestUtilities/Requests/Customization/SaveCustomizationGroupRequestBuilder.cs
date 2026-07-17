using Bogus;
using IOrder.Communication.Request;

namespace CommomTestUtilities.Requests.Customization;

public class SaveCustomizationGroupRequestBuilder
{
    public static SaveCustomizationGroupRequestDto Build()
    {
        return new Faker<SaveCustomizationGroupRequestDto>()
            .RuleFor(r => r.Name, f => f.Commerce.ProductName())
            .RuleFor(r => r.Type, "SingleChoice")
            .RuleFor(r => r.MinSelections, 1)
            .RuleFor(r => r.MaxSelections, 1)
            .RuleFor(r => r.Required, true)
            .RuleFor(r => r.Position, 0)
            .RuleFor(r => r.Options, f => [
                new SaveCustomizationOptionRequestDto
                {
                    Name = f.Commerce.ProductMaterial(),
                    PriceModifier = f.Finance.Amount(1, 10),
                    Position = 0
                }
            ])
            .Generate();
    }
}
