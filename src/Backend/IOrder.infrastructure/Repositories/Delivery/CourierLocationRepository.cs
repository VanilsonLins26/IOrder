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
