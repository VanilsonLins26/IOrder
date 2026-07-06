using CommomTestUtilities.Repositories;
using CommomTestUtilities.Requests.Coupon;
using IOrder.Application.UseCases.Coupon.Commands;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Coupon;

public class CreateCouponUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = CouponRequestBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Code.ShouldBe(request.Code);
        result.DiscountType.ShouldBe(request.DiscountType);
        result.DiscountValue.ShouldBe(request.DiscountValue);
    }

    [Fact]
    public async Task Error_Code_Already_Exists()
    {
        var request = CouponRequestBuilder.Build();
        var useCase = CreateUseCase(codeExists: true);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem().ShouldBe(ResourceMessagesException.COUPON_CODE_EXISTS);
    }

    [Fact]
    public async Task Error_Validation_Failed()
    {
        var request = CouponRequestBuilder.Build();
        request.Code = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
    }

    private static CreateCouponUseCase CreateUseCase(string? code = null, bool codeExists = false)
    {
        var readOnlyRepository = new CouponReadOnlyRepositoryBuilder();
        readOnlyRepository.CodeExistsAsync(codeExists);
        var writeOnlyRepository = new CouponWriteOnlyRepositoryBuilder().Create().Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var validator = new CreateCouponValidator();

        return new CreateCouponUseCase(
            readOnlyRepository.Build(),
            writeOnlyRepository,
            unitOfWork,
            validator);
    }
}
