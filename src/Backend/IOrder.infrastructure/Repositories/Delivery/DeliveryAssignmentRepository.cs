using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Delivery;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Repositories.Delivery;

internal class DeliveryAssignmentRepository : IDeliveryAssignmentReadOnlyRepository, IDeliveryAssignmentWriteOnlyRepository
{
    private readonly AppDbContext _context;

    public DeliveryAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DeliveryAssignment?> GetByIdAsync(Guid id)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .Include(a => a.Order)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<DeliveryAssignment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .Where(a => a.OrderId == orderId)
            .OrderByDescending(a => a.AssignedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IList<DeliveryAssignment>> GetByCourierUserIdAsync(string courierUserId, int pageNumber, int pageSize)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .Where(a => a.CourierUserId == courierUserId)
            .Include(a => a.Order)
                .ThenInclude(o => o.Store)
            .OrderByDescending(a => a.AssignedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByCourierUserIdAsync(string courierUserId)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .CountAsync(a => a.CourierUserId == courierUserId);
    }

    public async Task<IList<DeliveryAssignment>> GetPendingByStoreIdAsync(Guid storeId, int pageNumber, int pageSize)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .Where(a => a.Order.StoreId == storeId && a.Status == Domain.Entities.Enums.AssignmentStatus.Pending)
            .Include(a => a.Order)
            .OrderByDescending(a => a.AssignedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountByStoreIdAsync(Guid storeId)
    {
        return await _context.DeliveryAssignments
            .AsNoTracking()
            .CountAsync(a => a.Order.StoreId == storeId && a.Status == Domain.Entities.Enums.AssignmentStatus.Pending);
    }

    public async Task<DeliveryAssignment> CreateAsync(DeliveryAssignment assignment)
    {
        await _context.DeliveryAssignments.AddAsync(assignment);
        return assignment;
    }

    public DeliveryAssignment Update(DeliveryAssignment assignment)
    {
        _context.DeliveryAssignments.Update(assignment);
        return assignment;
    }

    public async Task<DeliveryAssignment?> GetByIdTrackingAsync(Guid id)
    {
        return await _context.DeliveryAssignments
            .Include(a => a.Order)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
