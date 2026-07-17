using System;
using CommomTestUtilities.Requests.Payment;
using IOrder.Application.UseCases.Payment.Commands;
using IOrder.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.Payment;

public class CreatePaymentValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build(Guid.NewGuid());
        
        var result = validator.Validate(request);
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_OrderId_Empty()
    {
        var validator = new CreatePaymentValidator();
        var request = CreatePaymentRequestBuilder.Build(Guid.Empty);
        request.OrderId = Guid.Empty;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PAYMENT_ORDER_ID_EMPTY);
    }
}
