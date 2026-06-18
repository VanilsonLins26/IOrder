using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Product;

public class CreatePromotionPriceUseCase : ICreatePromotionPriceUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;

    public CreatePromotionPriceUseCase(IProductWriteOnlyRepository writeOnlyRepository, IProductReadOnlyRepository readOnlyRepository, IUnitOfWork uof)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _uof = uof;
    }

    public async Task<PromotionPriceResponse> Execute(PromotionPriceResquestDto dto)
    {
        await Validate(dto);

        var promotionPrice = dto.Adapt<PromotionPrice>();

        var createdPromotionPrice = await _writeOnlyRepository.CreatePromotion(promotionPrice);

        return createdPromotionPrice.Adapt<PromotionPriceResponse>();
    }

    private async Task Validate(PromotionPriceResquestDto dto)
    {
        var validator = new CreatePromotionPriceValidator();
        var result = validator.Validate(dto);

        var product = await _readOnlyRepository.GetByIdAsync(dto.ProductId) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        if (dto.Price >= product.Price)
            throw new ErrorOnValidationException([ResourceMessagesException.PROMOTION_PRICE_INVALID]);

        var exitsPromotionInDate = await _readOnlyRepository.ExistsPromotionInDate(dto.InitialTime!.Value, dto.FinalTime!.Value);

        if (exitsPromotionInDate)
            throw new ErrorOnValidationException([ResourceMessagesException.EXISTS_PROMOTION_IN_THIS_DATE]);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
