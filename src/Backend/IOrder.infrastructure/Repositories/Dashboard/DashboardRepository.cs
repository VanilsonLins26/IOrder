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
        var pendingStatuses = new[] { 
            OrderStatus.Pending, 
            OrderStatus.Negotiating, 
            OrderStatus.AwaitingPayment, 
            OrderStatus.Paid, 
            OrderStatus.Preparing, 
            OrderStatus.Ready, 
            OrderStatus.OutForDelivery 
        };
        
        var pendingOrders = await orders.CountAsync(o => pendingStatuses.Contains(o.Status));
        var deliveredOrders = await orders.CountAsync(o => o.Status == OrderStatus.Delivered);

        // Revenue is calculated only for orders that are delivered or paid/preparing/ready/out for delivery
        var revenueStatuses = new[] {
            OrderStatus.Paid,
            OrderStatus.Preparing,
            OrderStatus.Ready,
            OrderStatus.OutForDelivery,
            OrderStatus.Delivered
        };

        var totalRevenue = await orders
            .Where(o => revenueStatuses.Contains(o.Status))
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
