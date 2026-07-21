using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories.Dashboard;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Dashboard.Queries;
using IOrder.Domain.Entities;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace UseCases.Test.Dashboard;

public class GetDashboardMetricsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var store = StoreBuilder.Build();
        var metrics = StoreDashboardSummaryBuilder.Build();
        var useCase = CreateUseCase(store, metrics);

        var response = await useCase.Execute();

        response.ShouldNotBeNull();
        response.TotalOrders.ShouldBe(metrics.TotalOrders);
        response.TotalRevenue.ShouldBe(metrics.TotalRevenue);
        response.PendingOrders.ShouldBe(metrics.PendingOrders);
        response.DeliveredOrders.ShouldBe(metrics.DeliveredOrders);
    }

    [Fact]
    public async Task Error_Store_Not_Found()
    {
        var metrics = StoreDashboardSummaryBuilder.Build();
        var useCase = CreateUseCase(null, metrics);

        Func<Task> act = async () => await useCase.Execute();

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe("Loja não encontrada.");
    }

    private GetDashboardMetricsUseCase CreateUseCase(IOrder.Domain.Entities.Store? store, StoreDashboardSummary metrics, string userId = "test-user-id")
    {
        var loggedUser = LoggedUserBuilder.Build(userId);
        
        var storeReadOnly = new StoreReadOnlyRepositoryBuilder();
        if (store is not null)
        {
            // I need to ensure store builder maps the user id properly
            store.UserId = userId; 
            storeReadOnly.GetByUserIdAsync(store);
        }
            
        var dashboardReadOnly = new DashboardReadOnlyRepositoryBuilder();
        dashboardReadOnly.GetStoreDashboardMetricsAsync(metrics);

        return new GetDashboardMetricsUseCase(
            loggedUser,
            storeReadOnly.Build(),
            dashboardReadOnly.Build());
    }
}
