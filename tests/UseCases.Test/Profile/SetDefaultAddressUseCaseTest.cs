using CommomTestUtilities.Entities.Profile;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Profile;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Profile.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Profile;

public class SetDefaultAddressUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var currentDefault = UserAddressBuilder.Build(isDefault: true);
        var newDefault = UserAddressBuilder.Build(isDefault: false);
        
        var addresses = new List<IOrder.Domain.Entities.UserAddress> { currentDefault, newDefault };
        var useCase = CreateUseCase(newDefault.Id, newDefault.UserId, newDefault, addresses);

        var act = async () => await useCase.ExecuteAsync(newDefault.Id);

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Address_Not_Found()
    {
        var useCase = CreateUseCase(Guid.NewGuid(), "test-user", null, new List<IOrder.Domain.Entities.UserAddress>());

        var act = async () => await useCase.ExecuteAsync(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldContain("Address not found.");
    }

    private SetDefaultAddressUseCase CreateUseCase(
        Guid addressId, 
        string userId, 
        IOrder.Domain.Entities.UserAddress? address, 
        List<IOrder.Domain.Entities.UserAddress> addresses)
    {
        var loggedUser = LoggedUserBuilder.Build(userId: userId);
        
        var readOnlyBuilder = new UserAddressReadOnlyRepositoryBuilder()
            .GetByIdAsync(addressId, address)
            .GetUserAddressesAsync(userId, addresses);
            
        var writeOnlyBuilder = new UserAddressWriteOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new SetDefaultAddressUseCase(
            loggedUser,
            readOnlyBuilder.Build(),
            writeOnlyBuilder.Build(),
            unitOfWork);
    }
}
