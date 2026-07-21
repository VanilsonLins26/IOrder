using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class ReviewBuilder
{
    public static OrderReview Build(Guid? orderId = null, Guid? storeId = null, string userId = "test-user-id", string? courierUserId = null)
    {
        return new Faker<OrderReview>("pt_BR")
            .CustomInstantiator(f => new OrderReview(
                orderId ?? f.Random.Guid(),
                storeId ?? f.Random.Guid(),
                userId,
                courierUserId,
                f.Random.Int(1, 5),
                courierUserId != null ? f.Random.Int(1, 5) : null,
                f.Lorem.Sentence()
            ))
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past())
            .Generate();
    }

    public static IList<OrderReview> BuildCollection(int count, Guid storeId)
    {
        var list = new List<OrderReview>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Build(storeId: storeId));
        }
        return list;
    }
}
