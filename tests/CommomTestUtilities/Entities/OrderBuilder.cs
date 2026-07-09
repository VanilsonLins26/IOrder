using Bogus;
using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;

namespace CommomTestUtilities.Entities;

public class OrderBuilder
{
    public static Order Build(Guid? storeId = null, string userId = "test-user-id")
    {
        var order = new Faker<Order>("pt_BR")
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.UserId, userId)
            .RuleFor(o => o.StoreId, f => storeId ?? f.Random.Guid())
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount(50, 1000))
            .RuleFor(o => o.OriginalAmount, (f, o) => o.TotalAmount)
            .RuleFor(o => o.CustomerNotes, f => f.Lorem.Sentence())
            .RuleFor(o => o.CustomerEmail, f => f.Internet.Email())
            .RuleFor(o => o.CreatedAt, DateTime.UtcNow)
            .Generate();

        foreach (var item in OrderItemBuilder.BuildCollection(2))
        {
            order.AddItem(item);
        }

        return order;
    }
}
