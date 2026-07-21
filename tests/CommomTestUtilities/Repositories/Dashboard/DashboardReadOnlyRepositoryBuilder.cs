using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Dashboard;
using Moq;
using System;

namespace CommomTestUtilities.Repositories.Dashboard;

public class DashboardReadOnlyRepositoryBuilder
{
    private readonly Mock<IDashboardReadOnlyRepository> _repository;

    public DashboardReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IDashboardReadOnlyRepository>();
    }

    public DashboardReadOnlyRepositoryBuilder GetStoreDashboardMetricsAsync(StoreDashboardSummary summary)
    {
        _repository.Setup(repo => repo.GetStoreDashboardMetricsAsync(It.IsAny<Guid>())).ReturnsAsync(summary);
        return this;
    }

    public IDashboardReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}
