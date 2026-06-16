using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Product;

public class CreateProductUseCase : ICreateProductUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;

    public CreateProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, IProductReadOnlyRepository readOnlyRepository)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<ProductResponseDto> Execute(ProductRequestDto productRequest)
    {
        await Validate(productRequest);

        var product = productRequest.Adapt<Domain.Entities.Product>();
        //product.StoreId = productRequest.storeId;
        var createdProduct = await _writeOnlyRepository.Create(product);

        await _uof.Commit();

        return createdProduct.Adapt<ProductResponseDto>();
    }

    private async Task Validate(ProductRequestDto productRequest)
    {
        var validator = new CreateProductValidator();
        var result = validator.Validate(productRequest);

        var nameExists = await _readOnlyRepository.NameExists(productRequest.Name);

        if (nameExists)
            throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }


    }
}
