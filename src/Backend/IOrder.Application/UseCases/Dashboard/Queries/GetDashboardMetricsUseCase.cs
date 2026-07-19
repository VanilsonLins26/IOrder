using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Dashboard;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Dashboard.Queries;

public class GetDashboardMetricsUseCase : IGetDashboardMetricsUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStoreReadOnlyRepository _storeReadOnlyRepository;
    private readonly IDashboardReadOnlyRepository _dashboardRepository;

    public GetDashboardMetricsUseCase(
        ILoggedUserService loggedUserService, 
        IStoreReadOnlyRepository storeReadOnlyRepository,
        IDashboardReadOnlyRepository dashboardRepository)
    {
        _loggedUserService = loggedUserService;
        _storeReadOnlyRepository = storeReadOnlyRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardMetricsResponseDto> Execute()
    {
        var userId = _loggedUserService.GetUserId();

        var store = await _storeReadOnlyRepository.GetByUserIdAsync(userId);

        if (store is null)
        {
            throw new NotFoundException(new System.Collections.Generic.List<string> { "Loja não encontrada." });
        }

        var metrics = await _dashboardRepository.GetStoreDashboardMetricsAsync(store.Id);

        return metrics.Adapt<DashboardMetricsResponseDto>();
    }
}
