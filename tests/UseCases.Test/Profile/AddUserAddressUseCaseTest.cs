using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Profile;
using CommomTestUtilities.Requests.Profile;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Profile.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Profile;

public class AddUserAddressUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = AddUserAddressRequestBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync(request);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(request.Name);
        result.IsDefault.ShouldBeTrue(); // First address is default
    }

    [Fact]
    public async Task Success_Not_Default()
    {
        var request = AddUserAddressRequestBuilder.Build();
        var useCase = CreateUseCase(addressCount: 1);

        var result = await useCase.ExecuteAsync(request);

        result.ShouldNotBeNull();
        result.IsDefault.ShouldBeFalse(); // Already has an address
    }

    [Fact]
    public async Task Error_Max_Addresses_Limit()
    {
        var request = AddUserAddressRequestBuilder.Build();
        var useCase = CreateUseCase(addressCount: 10);

        var act = async () => await useCase.ExecuteAsync(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldContain("Maximum of 10 addresses allowed.");
    }

    private AddUserAddressUseCase CreateUseCase(int addressCount = 0)
    {
        var loggedUser = LoggedUserBuilder.Build(userId: "test-user");
        
        var readOnlyBuilder = new UserAddressReadOnlyRepositoryBuilder()
            .CountUserAddressesAsync("test-user", addressCount);
            
        var writeOnlyBuilder = new UserAddressWriteOnlyRepositoryBuilder();
        var geocodingBuilder = new GeocodingServiceBuilder()
            .GetCoordinatesAsync(-23.5505, -46.6333);
        
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new AddUserAddressUseCase(
            loggedUser,
            readOnlyBuilder.Build(),
            writeOnlyBuilder.Build(),
            geocodingBuilder.Build(),
            unitOfWork);
    }
}
