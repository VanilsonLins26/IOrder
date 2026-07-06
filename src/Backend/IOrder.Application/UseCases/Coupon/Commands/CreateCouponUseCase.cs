using FluentValidation;
using IOrder.Communication.Response;
using IOrder.Domain.Repositories;
using IOrder.Domain.Repositories.Coupon;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Mapster;

namespace IOrder.Application.UseCases.Coupon.Commands;

public class CreateCouponUseCase : ICreateCouponUseCase
{
    private readonly ICouponReadOnlyRepository _readOnlyRepository;
    private readonly ICouponWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Communication.Request.CouponRequestDto> _validator;

    public CreateCouponUseCase(
        ICouponReadOnlyRepository readOnlyRepository,
        ICouponWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork,
        IValidator<Communication.Request.CouponRequestDto> validator)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CouponResponseDto> Execute(Communication.Request.CouponRequestDto request)
    {
        await Validate(request);

        var coupon = request.Adapt<Domain.Entities.Coupon>();

        var created = await _writeOnlyRepository.Create(coupon);
        await _unitOfWork.Commit();

        return created.Adapt<CouponResponseDto>();
    }

    private async Task Validate(Communication.Request.CouponRequestDto request)
    {
        var result = await _validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }

        var codeExists = await _readOnlyRepository.CodeExistsAsync(request.Code);
        if (codeExists)
            throw new ErrorOnValidationException([ResourceMessagesException.COUPON_CODE_EXISTS]);
    }
}
