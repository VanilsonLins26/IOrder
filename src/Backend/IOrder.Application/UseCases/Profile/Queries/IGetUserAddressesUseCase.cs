using IOrder.Communication.Response.Profile;

namespace IOrder.Application.UseCases.Profile.Queries;

public interface IGetUserAddressesUseCase
{
    Task<List<UserAddressResponseDto>> ExecuteAsync();
}
