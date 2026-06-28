using CommomTestUtilities.Requests.Product;
using IOrder.Application.UseCases.Product;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Product;

public class UpdateProductValidatorTest
{

    [Fact]
    public void Success()
    {
        var validator = new UpdateProductValidator();

        var request = RequestUpdateProductBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new UpdateProductValidator();

        var request = RequestUpdateProductBuilder.Build();
        request.Name = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);

    }

    [Fact]
    public void Error_Price_Empty()
    {
        var validator = new UpdateProductValidator();

        var request = RequestUpdateProductBuilder.Build();
        request.Price = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_EMPTY);

    }

    [Fact]
    public void Error_Price_Less_Than_Zero()
    {
        var validator = new UpdateProductValidator();

        var request = RequestUpdateProductBuilder.Build();
        request.Price = -5;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_GREATER_THAN_0);

    }

   
}
