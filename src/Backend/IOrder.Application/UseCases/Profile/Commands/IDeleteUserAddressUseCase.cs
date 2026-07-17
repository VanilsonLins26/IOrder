namespace IOrder.Application.UseCases.Profile.Commands;

public interface IDeleteUserAddressUseCase
{
    Task ExecuteAsync(Guid addressId);
}
