using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using FluentValidation;

namespace IOrder.Application.UseCases.Product.Commands;

public class CreateProductUseCase : ICreateProductUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IValidator<ProductRequestDto> _validator;

    public CreateProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, IProductReadOnlyRepository readOnlyRepository, IStorePermissionService storePermissionService, IValidator<ProductRequestDto> validator)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _readOnlyRepository = readOnlyRepository;
        _storePermissionService = storePermissionService;
        _validator = validator;
    }

    public async Task<ProductResponseDto> Execute(ProductRequestDto productRequest)
    {
        await Validate(productRequest);

        var product = productRequest.Adapt<Domain.Entities.Product>();
        product.UpdatePrice(productRequest.Price.GetValueOrDefault());
        product.StoreId = await _storePermissionService.GetLoggedUserStoreIdAsync();
        var createdProduct = await _writeOnlyRepository.Create(product);

        await _uof.Commit();

        return createdProduct.Adapt<ProductResponseDto>();
    }

    private async Task Validate(ProductRequestDto productRequest)
    {
        var result = await _validator.ValidateAsync(productRequest);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

        var nameExists = await _readOnlyRepository.NameExists(productRequest.Name);


        if (nameExists)
            throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);

        


    }
}

