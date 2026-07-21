using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class StoreDashboardSummaryBuilder
{
    public static StoreDashboardSummary Build()
    {
        return new Faker<StoreDashboardSummary>()
            .RuleFor(s => s.TotalOrders, f => f.Random.Int(10, 100))
            .RuleFor(s => s.TotalRevenue, f => f.Random.Decimal(100, 10000))
            .RuleFor(s => s.PendingOrders, f => f.Random.Int(1, 10))
            .RuleFor(s => s.DeliveredOrders, f => f.Random.Int(5, 50));
    }
}
