using CommomTestUtilities.Entities;
using CommomTestUtilities.Repositories.Delivery;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Delivery.Queries;
using IOrder.Domain.Entities;
using Shouldly;

namespace UseCases.Test.Delivery;

public class GetMyDeliveriesUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var courierId = "test-courier";
        var assignment = DeliveryAssignmentBuilder.Build(courierUserId: courierId, orderId: Guid.NewGuid());
        var assignments = new List<DeliveryAssignment> { assignment };
        var location = CourierLocationBuilder.Build(courierId);
        var useCase = CreateUseCase(courierId, assignments, 1, location);

        var response = await useCase.Execute(1, 10);

        response.ShouldNotBeNull();
        response.Items.ShouldNotBeNull();
        response.Items.Count.ShouldBe(1);
        response.TotalCount.ShouldBe(1);
        response.Items.First().CourierUserId.ShouldBe(courierId);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var courierId = "test-courier";
        var useCase = CreateUseCase(courierId, new List<DeliveryAssignment>(), 0, null);

        var response = await useCase.Execute(1, 10);

        response.ShouldNotBeNull();
        response.Items.ShouldBeEmpty();
        response.TotalCount.ShouldBe(0);
    }

    private GetMyDeliveriesUseCase CreateUseCase(string courierId, List<DeliveryAssignment> assignments, int totalCount, CourierLocation? location)
    {
        var loggedUser = LoggedUserBuilder.Build(courierId);
        
        var assignmentReadOnly = new DeliveryAssignmentReadOnlyRepositoryBuilder()
            .GetByCourierUserIdAsync(assignments)
            .GetCountByCourierUserIdAsync(totalCount)
            .Build();

        var locationReadOnly = new CourierLocationReadOnlyRepositoryBuilder()
            .GetByCourierUserIdAsync(location)
            .Build();

        return new GetMyDeliveriesUseCase(
            loggedUser,
            assignmentReadOnly,
            locationReadOnly);
    }
}
