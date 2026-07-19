using IOrder.Domain.Entities;
using IOrder.Domain.Entities.Enums;
using IOrder.Domain.Repositories.Dashboard;
using IOrder.infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IOrder.infrastructure.Repositories.Dashboard;

internal class DashboardRepository : IDashboardReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public DashboardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreDashboardSummary> GetStoreDashboardMetricsAsync(Guid storeId)
    {
        var orders = _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.StoreId == storeId);

        var totalOrders = await orders.CountAsync();

        // Consider pending as everything that is not delivered, cancelled or declined
        var pendingOrders = await orders.CountAsync(o => 
            o.Status != OrderStatus.Delivered && 
            o.Status != OrderStatus.Cancelled && 
            o.Status != OrderStatus.Declined);

        var deliveredOrders = await orders.CountAsync(o => o.Status == OrderStatus.Delivered);

        // Revenue is calculated only for orders that are delivered or paid/preparing/ready/out for delivery
        var totalRevenue = await orders
            .Where(o => o.Status == OrderStatus.Paid ||
                        o.Status == OrderStatus.Preparing ||
                        o.Status == OrderStatus.Ready ||
                        o.Status == OrderStatus.OutForDelivery ||
                        o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount);

        return new StoreDashboardSummary
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            PendingOrders = pendingOrders,
            DeliveredOrders = deliveredOrders
        };
    }
}
