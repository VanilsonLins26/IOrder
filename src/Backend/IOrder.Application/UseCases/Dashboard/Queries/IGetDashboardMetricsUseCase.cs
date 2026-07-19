using IOrder.Communication.Response;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Dashboard.Queries;

public interface IGetDashboardMetricsUseCase
{
    Task<DashboardMetricsResponseDto> Execute();
}
