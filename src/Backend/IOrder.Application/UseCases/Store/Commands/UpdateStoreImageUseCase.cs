using IOrder.Application.Services.StorePermission;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOrder.Application.UseCases.Store.Commands;

public class UpdateStoreImageUseCase : IUpdateStoreImageUseCase
{
    private readonly IStoreReadOnlyRepository _readOnlyRepository;
    private readonly IStoreWriteOnlyRepository _writeOnlyRepository;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateStoreImageUseCase(
        IStoreReadOnlyRepository readOnlyRepository,
        IStoreWriteOnlyRepository writeOnlyRepository,
        IStorePermissionService storePermissionService,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _storePermissionService = storePermissionService;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }
    public async Task<string> Execute(Stream fileStream, string fileName)
    {

        var storeId = await _storePermissionService.GetLoggedUserStoreIdAsync();
        await _storePermissionService.ValidateStoreOwnerAsync(storeId);
        var store = await _writeOnlyRepository.GetByIdTracking(storeId);
        var imageUrl = await _storageService.UploadImageAsync(fileStream, fileName);

        if (!string.IsNullOrEmpty(store.ImageUrl))
        {
            await _storageService.DeleteImageAsync(store.ImageUrl);
        }
        store.ImageUrl = imageUrl;
        _writeOnlyRepository.Update(store);
        await _unitOfWork.Commit();
        return imageUrl;
    }
}