using CommomTestUtilities.Requests.Product;
using IOrder.Application.UseCases.Product;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Product;

public class CreateProductValidatorTest
{

    [Fact]
    public void Success()
    {
        var validator = new CreateProductValidator();

        var request = RequestCreateProductBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Error_Name_Empty()
    {
        var validator = new CreateProductValidator();

        var request = RequestCreateProductBuilder.Build();
        request.Name = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.NAME_EMPTY);

    }

    [Fact]
    public void Error_Price_Empty()
    {
        var validator = new CreateProductValidator();

        var request = RequestCreateProductBuilder.Build();
        request.Price = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_EMPTY);

    }

    [Fact]
    public void Error_Price_Less_Than_Zero()
    {
        var validator = new CreateProductValidator();

        var request = RequestCreateProductBuilder.Build();
        request.Price = -5;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_GREATER_THAN_0);

    }

    [Fact]
    public void Error_Unit_Of_Measure_Empty()
    {
        var validator = new CreateProductValidator();

        var request = RequestCreateProductBuilder.Build();
        request.UnitOfMeasure = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.UNIT_OF_MEASURE_EMPTY);

    }
}
