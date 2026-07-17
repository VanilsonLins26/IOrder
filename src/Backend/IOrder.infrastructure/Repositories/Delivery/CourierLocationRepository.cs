using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IOrder.infrastructure.Repositories.Delivery;

internal class CourierLocationRepository : ICourierLocationReadOnlyRepository, ICourierLocationWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public CourierLocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourierLocation?> GetByCourierUserIdAsync(string courierUserId)
    {
        return await _context.CourierLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourierUserId == courierUserId);
    }

    public async Task<IList<CourierLocation>> GetAllActiveAsync()
    {
        return await _context.CourierLocations
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AvailableCourier>> GetAvailableCouriersAsync(
        double storeLat, double storeLon, double radiusKm, Guid excludeOrderId)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-30);
        var radiusMeters = radiusKm * 1000.0;
        var storePoint = new Point(storeLon, storeLat) { SRID = 4326 };

        var activeOrderCourierIds = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id != excludeOrderId
                && o.Status == Domain.Entities.Enums.OrderStatus.OutForDelivery
                && o.ActiveAssignment != null)
            .Select(o => o.ActiveAssignment!.CourierUserId)
            .ToListAsync();

        var candidates = await _context.CourierLocations
            .AsNoTracking()
            .Where(cl => cl.Location != null
                && cl.UpdatedAt >= cutoff)
            .ToListAsync();

        var result = new List<AvailableCourier>();

        foreach (var cl in candidates)
        {
            if (activeOrderCourierIds.Contains(cl.CourierUserId))
                continue;

            if (cl.Location == null) continue;

            var distMeters = storePoint.Distance(cl.Location) * 111320.0;
            var distKm = distMeters / 1000.0;

            if (distKm <= radiusKm)
            {
                result.Add(new AvailableCourier
                {
                    CourierUserId = cl.CourierUserId,
                    DistanceKm = Math.Round(distKm, 2),
                    LastLocationAt = cl.UpdatedAt
                });
            }
        }

        return result.OrderBy(c => c.DistanceKm).ToList();
    }

    public async Task UpsertAsync(CourierLocation location)
    {
        var existing = await _context.CourierLocations
            .FirstOrDefaultAsync(c => c.CourierUserId == location.CourierUserId);

        location.Location = new Point(location.Longitude, location.Latitude) { SRID = 4326 };

        if (existing is null)
        {
            await _context.CourierLocations.AddAsync(location);
        }
        else
        {
            existing.Latitude = location.Latitude;
            existing.Longitude = location.Longitude;
            existing.Location = location.Location;
            existing.UpdatedAt = location.UpdatedAt;
        }
    }
}
