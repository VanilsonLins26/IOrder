using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Product;

public class UpdateProductUseCase : IUpdateProductUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;

    public UpdateProductUseCase(IProductWriteOnlyRepository writeOnlyRepository, IUnitOfWork uof, IProductReadOnlyRepository readOnlyRepository)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _uof = uof;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<ProductResponseDto> Execute(Guid id, UpdateProductRequestDto dto)
    {
        var product = await _writeOnlyRepository.GetByIdTracking(id) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        await Validate(dto, product.Name);

        dto.Adapt(product);

        await _uof.Commit();

        return product.Adapt<ProductResponseDto>();

    }

  

    private async Task Validate(UpdateProductRequestDto productRequest, string productName)
    {
        var validator = new UpdateProductValidator();
        var result = validator.Validate(productRequest);

        var nameExists = await _readOnlyRepository.NameExists(productRequest.Name);

        if (nameExists && productRequest.Name != productName)
            throw new ErrorOnValidationException([ResourceMessagesException.NAME_ALREADY_EXISTS]);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }


    }
}
