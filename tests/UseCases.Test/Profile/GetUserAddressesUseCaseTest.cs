using CommomTestUtilities.Entities.Profile;
using CommomTestUtilities.Mapper;
using CommomTestUtilities.Repositories.Profile;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Profile.Queries;
using Shouldly;

namespace UseCases.Test.Profile;

public class GetUserAddressesUseCaseTest
{
    public GetUserAddressesUseCaseTest()
    {
        MapsterSettings.Configure();
    }

    [Fact]
    public async Task Success()
    {
        var address = UserAddressBuilder.Build();
        var useCase = CreateUseCase(address.UserId, new List<IOrder.Domain.Entities.UserAddress> { address });

        var result = await useCase.ExecuteAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(address.Id);
        result[0].Name.ShouldBe(address.Name);
    }

    [Fact]
    public async Task Success_Empty()
    {
        var useCase = CreateUseCase("test-user", new List<IOrder.Domain.Entities.UserAddress>());

        var result = await useCase.ExecuteAsync();

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private GetUserAddressesUseCase CreateUseCase(string userId, List<IOrder.Domain.Entities.UserAddress> addresses)
    {
        var loggedUser = LoggedUserBuilder.Build(userId: userId);
        
        var readOnlyBuilder = new UserAddressReadOnlyRepositoryBuilder()
            .GetUserAddressesAsync(userId, addresses);

        return new GetUserAddressesUseCase(
            loggedUser,
            readOnlyBuilder.Build());
    }
}
