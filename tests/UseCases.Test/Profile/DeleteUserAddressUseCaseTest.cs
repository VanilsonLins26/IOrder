using CommomTestUtilities.Entities.Profile;
using CommomTestUtilities.Repositories;
using CommomTestUtilities.Repositories.Profile;
using CommomTestUtilities.Services;
using IOrder.Application.UseCases.Profile.Commands;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;

namespace UseCases.Test.Profile;

public class DeleteUserAddressUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var address = UserAddressBuilder.Build();
        var useCase = CreateUseCase(address.Id, address.UserId, address);

        var act = async () => await useCase.ExecuteAsync(address.Id);

        await act.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Error_Address_Not_Found()
    {
        var useCase = CreateUseCase(Guid.NewGuid(), "test-user", null);

        var act = async () => await useCase.ExecuteAsync(Guid.NewGuid());

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldContain("Address not found.");
    }

    private DeleteUserAddressUseCase CreateUseCase(Guid addressId, string userId, IOrder.Domain.Entities.UserAddress? address)
    {
        var loggedUser = LoggedUserBuilder.Build(userId: userId);
        
        var readOnlyBuilder = new UserAddressReadOnlyRepositoryBuilder()
            .GetByIdAsync(addressId, address);
            
        var writeOnlyBuilder = new UserAddressWriteOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new DeleteUserAddressUseCase(
            loggedUser,
            readOnlyBuilder.Build(),
            writeOnlyBuilder.Build(),
            unitOfWork);
    }
}
