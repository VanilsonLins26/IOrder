namespace IOrder.Domain.Entities;

public class StoreDashboardSummary
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }
}
