using IOrder.Application.Services.LoggedUser;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories.Store;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.Services.StorePermission;

public class StorePermissionService : IStorePermissionService
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly IStoreReadOnlyRepository _reaOnlyRepository;

    public StorePermissionService(ILoggedUserService loggedUserService, IStoreReadOnlyRepository reaOnlyRepository)
    {
        _loggedUserService = loggedUserService;
        _reaOnlyRepository = reaOnlyRepository;
    }

    public async Task<Guid> GetLoggedUserStoreIdAsync()
    {
        var userId = _loggedUserService.GetUserId();
        var store = await _reaOnlyRepository.GetByUserIdAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.STORE_NOT_FOUND]);

        return store.Id;
    }

    public async Task ValidateProductOwnershipAsync(Product product)
    {
        var loggedStoreId = await GetLoggedUserStoreIdAsync();
        if (product.StoreId != loggedStoreId)
            throw new UnauthorizedStoreException([ResourceMessagesException.UNAUTHORIZED_STORE]);
    }

    public Task ValidateStoreOwnerAsync(Store store) 
    {
        var userId = _loggedUserService.GetUserId();
        if (store.UserId != userId)
            throw new UnauthorizedStoreException([ResourceMessagesException.UNAUTHORIZED_STORE]);

        return Task.CompletedTask;
    }
}
