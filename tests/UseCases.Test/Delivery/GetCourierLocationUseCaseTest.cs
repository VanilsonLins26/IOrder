using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories.Delivery;
using IOrder.Application.UseCases.Delivery.Queries;
using IOrder.Domain.Entities;
using Shouldly;

namespace UseCases.Test.Delivery;

public class GetCourierLocationUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var courierId = "test-courier";
        var location = CourierLocationBuilder.Build(courierId);
        var useCase = CreateUseCase(courierId, location);

        var response = await useCase.Execute(courierId);

        response.ShouldNotBeNull();
        response.CourierUserId.ShouldBe(courierId);
    }

    [Fact]
    public async Task Success_NoLocation()
    {
        var courierId = "test-courier";
        var useCase = CreateUseCase(courierId, null);

        var response = await useCase.Execute(courierId);

        response.ShouldBeNull();
    }

    private GetCourierLocationUseCase CreateUseCase(string courierId, CourierLocation? location)
    {
        var locationReadOnly = new CourierLocationReadOnlyRepositoryBuilder()
            .GetByCourierUserIdAsync(location)
            .Build();

        return new GetCourierLocationUseCase(locationReadOnly);
    }
}
