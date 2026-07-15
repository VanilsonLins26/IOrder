namespace IOrder.Application.UseCases.Profile.Commands;

public interface ISetDefaultAddressUseCase
{
    Task ExecuteAsync(Guid addressId);
}
