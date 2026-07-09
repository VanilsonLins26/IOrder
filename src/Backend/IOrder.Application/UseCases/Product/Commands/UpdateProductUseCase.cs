using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using FluentValidation;

namespace IOrder.Application.UseCases.Product.Commands;

public class UpdateProductUseCase : IUpdateProductUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IValidator<UpdateProductRequestDto> _validator;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UpdateProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, IProductReadOnlyRepository readOnlyRepository, IStorePermissionService storePermissionService, IValidator<UpdateProductRequestDto> validator, IDomainEventDispatcher domainEventDispatcher)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _readOnlyRepository = readOnlyRepository;
        _storePermissionService = storePermissionService;
        _validator = validator;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<ProductResponseDto> Execute(Guid id, UpdateProductRequestDto dto)
    {
        var product = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        await _storePermissionService.ValidateProductOwnershipAsync(product);

        await Validate(dto, product.Name);

        dto.Adapt(product);
        product.UpdatePrice(dto.Price.GetValueOrDefault());

        await _uof.Commit();

        await _domainEventDispatcher.DispatchAsync(product.DomainEvents);
        product.ClearDomainEvents();

        return product.Adapt<ProductResponseDto>();

    }

  

    private async Task Validate(UpdateProductRequestDto productRequest, string productName)
    {
        var result = await _validator.ValidateAsync(productRequest);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }

        if (productRequest.Name != productName)
        {
            var nameExists = await _readOnlyRepository.NameExists(productRequest.Name);
            if (nameExists)
                throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);
        }
    }
}

