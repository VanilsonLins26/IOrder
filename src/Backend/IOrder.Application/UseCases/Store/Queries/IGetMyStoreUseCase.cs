using IOrder.Communication.Response;

namespace IOrder.Application.UseCases.Store.Queries;

public interface IGetMyStoreUseCase
{
    Task<StoreResponseDto> Execute();
}

