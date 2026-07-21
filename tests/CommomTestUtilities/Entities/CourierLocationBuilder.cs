using Bogus;
using IOrder.Domain.Entities;

namespace CommomTestUtilities.Entities;

public class CourierLocationBuilder
{
    public static CourierLocation Build(string? courierUserId = null)
    {
        return new Faker<CourierLocation>()
            .RuleFor(l => l.CourierUserId, courierUserId ?? "test-courier-id")
            .RuleFor(l => l.Location, f => new NetTopologySuite.Geometries.Point(f.Address.Longitude(), f.Address.Latitude()) { SRID = 4326 })
            .RuleFor(l => l.UpdatedAt, f => DateTime.UtcNow)
            .Generate();
    }
}
