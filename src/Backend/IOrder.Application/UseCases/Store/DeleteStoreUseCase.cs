using IOrder.Application.Services.LoggedUser;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store;

public class DeleteStoreUseCase : IDeleteStoreUseCase
{
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly ILoggedUserService _loggedUserService;

    public DeleteStoreUseCase(IStoreWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, ILoggedUserService loggedUserService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _loggedUserService = loggedUserService;
    }

    public async Task<StoreResponseDto> Execute(Guid id)
    {
        var userId = _loggedUserService.GetUserId();
        var store = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        if (userId != store.UserId)
            throw new UnauthorizedAccessException(ResourceMessagesException.UNAUTHORIZED_STORE);

        _writeOnlyRepository.Delete(store);

        await _uof.Commit();

        return store.Adapt<StoreResponseDto>();
    }
}
