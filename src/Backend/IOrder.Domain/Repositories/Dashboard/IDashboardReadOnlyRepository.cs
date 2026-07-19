using IOrder.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace IOrder.Domain.Repositories.Dashboard;

public interface IDashboardReadOnlyRepository
{
    Task<StoreDashboardSummary> GetStoreDashboardMetricsAsync(Guid storeId);
}
