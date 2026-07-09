using IOrder.Application.Services.StorePermission;
using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Entities;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Product;
using IOrder.Domain.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;
using FluentValidation;

namespace IOrder.Application.UseCases.Product.Commands;

public class CreatePromotionPriceUseCase : ICreatePromotionPriceUseCase
{
    private readonly IProductWriteOnlyRepository _writeOnlyRepository;
    private readonly IProductReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _uof;
    private readonly IStorePermissionService _storePermissionService;
    private readonly IValidator<PromotionPriceResquestDto> _validator;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public CreatePromotionPriceUseCase(IProductWriteOnlyRepository writeOnlyRepository, IProductReadOnlyRepository readOnlyRepository, IUnitOfWork uof, IStorePermissionService storePermissionService, IValidator<PromotionPriceResquestDto> validator, IDomainEventDispatcher domainEventDispatcher)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _uof = uof;
        _storePermissionService = storePermissionService;
        _validator = validator;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<PromotionPriceResponseDto> Execute(PromotionPriceResquestDto dto)
    {
        await Validate(dto);

        var promotionPrice = dto.Adapt<PromotionPrice>();

        var createdPromotionPrice = await _writeOnlyRepository.CreatePromotion(promotionPrice);
        await _uof.Commit();

        var product = await _readOnlyRepository.GetByIdAsync(dto.ProductId);
        if (product is not null)
        {
            var promoEvent = new IOrder.Domain.Events.PromotionActivatedEvent(
                createdPromotionPrice.Id, product.Id, product.Name, createdPromotionPrice.Price);
            await _domainEventDispatcher.DispatchAsync([promoEvent]);
        }

        return createdPromotionPrice.Adapt<PromotionPriceResponseDto>();
    }

    private async Task Validate(PromotionPriceResquestDto dto)
    {
        var result = await _validator.ValidateAsync(dto);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }

        var product = await _readOnlyRepository.GetByIdAsync(dto.ProductId) ?? throw new NotFoundException([ResourceMessagesException.PRODUCT_NOT_FOUND]);

        await _storePermissionService.ValidateProductOwnershipAsync(product);

        if (dto.Price >= product.Price)
            throw new ErrorOnValidationException([ResourceMessagesException.PROMOTION_PRICE_INVALID]);

        var exitsPromotionInDate = await _readOnlyRepository.ExistsPromotionInDate(dto.ProductId, dto.InitialTime!.Value, dto.FinalTime!.Value);

        if (exitsPromotionInDate)
            throw new ErrorOnValidationException([ResourceMessagesException.EXISTS_PROMOTION_IN_THIS_DATE]);

 
    }


}

