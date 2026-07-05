using IOrder.Communication.Request;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories.Cart;
using IOrder.Domain.Security.Services;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Cart.Commands;

public class ApplyCouponUseCase : IApplyCouponUseCase
{
    private readonly ILoggedUserService _loggedUserService;
    private readonly ICartReadOnlyRepository _readOnlyRepositoy;
    private readonly ICartWriteOnlyRepository _writeOnlyRepository;
    private readonly FluentValidation.IValidator<ApplyCouponRequestDto> _validator;

    public ApplyCouponUseCase(
        ILoggedUserService loggedUserService, 
        ICartReadOnlyRepository readOnlyRepositoy, 
        ICartWriteOnlyRepository writeOnlyRepository,
        FluentValidation.IValidator<ApplyCouponRequestDto> validator)
    {
        _loggedUserService = loggedUserService;
        _readOnlyRepositoy = readOnlyRepositoy;
        _writeOnlyRepository = writeOnlyRepository;
        _validator = validator;
    }

    public async Task<CartResponseDto> Execute(ApplyCouponRequestDto request)
    {
        await Validate(request);

        var userId = _loggedUserService.GetUserId();

        var cart = await _readOnlyRepositoy.GetCartAsync(userId) ?? throw new NotFoundException([ResourceMessagesException.INVALID_CART]);

        cart.CouponCode = request.CouponCode;

        await _writeOnlyRepository.SaveCartAsync(cart);

        return cart.Adapt<CartResponseDto>();
    }

    private async Task Validate(ApplyCouponRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
