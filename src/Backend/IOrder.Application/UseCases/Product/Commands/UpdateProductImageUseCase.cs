using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Repositories.Store;
using IOrder.Domain.Services.LoggedUser;
using IOrder.Domain.Services.Storage;
using IOrder.Exceptions.ExceptionsBase;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IOrder.Application.UseCases.Product.Commands;

public class UpdateProductImageUseCase : IUpdateProductImageUseCase
{
    private readonly IProductReadOnlyRepository _productReadOnlyRepository;
    private readonly IProductWriteOnlyRepository _productWriteOnlyRepository;
    private readonly IStoreReadOnlyRepository _storeReadOnlyRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductImageUseCase(
        IProductReadOnlyRepository productReadOnlyRepository,
        IProductWriteOnlyRepository productWriteOnlyRepository,
        IStoreReadOnlyRepository storeReadOnlyRepository,
        ILoggedUser loggedUser,
        IStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _productReadOnlyRepository = productReadOnlyRepository;
        _productWriteOnlyRepository = productWriteOnlyRepository;
        _storeReadOnlyRepository = storeReadOnlyRepository;
        _loggedUser = loggedUser;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Execute(Guid productId, Stream fileStream, string fileName)
    {
        var user = await _loggedUser.User();
        var store = await _storeReadOnlyRepository.GetByUserIdAsync(user.Id);
        
        if (store == null)
            throw new NotFoundException("Loja não encontrada para este usuário.");

        var product = await _productReadOnlyRepository.GetById(productId);

        // Segurança: O produto tem que existir E tem que pertencer à loja do usuário logado
        if (product == null || product.StoreId != store.Id)
        {
            throw new NotFoundException("Produto não encontrado ou não pertence a sua loja.");
        }

        // Apaga a foto antiga se tiver
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            await _storageService.DeleteImageAsync(product.ImageUrl);
        }

        // Sobe a foto nova
        var imageUrl = await _storageService.UploadImageAsync(fileStream, fileName);

        // Atualiza e salva
        product.ImageUrl = imageUrl;
        _productWriteOnlyRepository.Update(product);
        await _unitOfWork.Commit();

        return imageUrl;
    }
}
