using IOrder.Domain.Security.Services;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Store.Queries;

public class GetMyStoreUseCase : IGetMyStoreUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUserService _loggedUserService;
    public GetMyStoreUseCase(IStoreReadOnlyRepository readOnlyRepository, ILoggedUserService loggedUserService)
    {
        _readOnlyRepository = readOnlyRepository;
        _loggedUserService = loggedUserService;
    }
    public async Task<StoreResponseDto> Execute()
    {
        var userId = _loggedUserService.GetUserId();
        var store = await _readOnlyRepository.GetByUserIdAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);
        return store.Adapt<StoreResponseDto>();
    }
}


