using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class OrderItemBuilder
{
    public static List<OrderItem> BuildCollection(int count = 3)
    {
        return new Faker<OrderItem>("pt_BR")
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.ProductImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(5, 500))
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
            .Generate(count);
    }
}
