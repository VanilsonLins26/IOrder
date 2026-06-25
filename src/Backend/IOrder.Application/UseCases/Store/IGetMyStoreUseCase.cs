using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Store;

public interface IGetMyStoreUseCase
{
    Task<StoreResponseDto> Execute();
}
