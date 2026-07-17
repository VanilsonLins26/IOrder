using IOrder.Communication.Request.Profile;
using IOrder.Communication.Response.Profile;

namespace IOrder.Application.UseCases.Profile.Commands;

public interface IAddUserAddressUseCase
{
    Task<UserAddressResponseDto> ExecuteAsync(AddUserAddressRequestDto request);
}
