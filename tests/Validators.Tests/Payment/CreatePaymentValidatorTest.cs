using CommomTestUtilities.Requests.Payment;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Communication.Enums;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Payment;

public class CreatePaymentValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildPix();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_CreditCard()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildCreditCard();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Success_Boleto()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildBoleto();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_OrderId_Empty()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build();
        request.OrderId = Guid.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_ORDER_ID_EMPTY);
    }

    [Fact]
    public void Error_Method_Invalid()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build();
        request.Method = (PaymentMethodDto)999;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_METHOD_INVALID);
    }

    [Fact]
    public void Error_PayerEmail_Empty()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build();
        request.PayerEmail = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessagesException.PAYMENT_EMAIL_EMPTY);
    }

    [Fact]
    public void Error_PayerEmail_Invalid()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build();
        request.PayerEmail = "not-an-email";

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == ResourceMessagesException.PAYMENT_EMAIL_INVALID);
    }

    [Fact]
    public void Error_CardToken_Empty()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildCreditCard();
        request.CardToken = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_CARD_TOKEN_EMPTY);
    }

    [Fact]
    public void Error_Installments_Null()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildCreditCard();
        request.Installments = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_INSTALLMENTS_EMPTY);
    }

    [Fact]
    public void Error_Installments_Zero()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.BuildCreditCard();
        request.Installments = 0;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_INSTALLMENTS_INVALID);
    }
}
