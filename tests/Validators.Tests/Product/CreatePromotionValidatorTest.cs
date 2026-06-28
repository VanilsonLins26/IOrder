using CommomTestUtilities.Requests.Product;
using IOrder.Application.UseCases.Product;
using IOrder.Exceptions;
using Shouldly;

namespace Validators.Tests.Product;

public class CreatePromotionValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void Product_Id_Empty()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.ProductId = default;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRODUCT_ID_EMPTY);

    }

    [Fact]
    public void Error_Price_Empty()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.Price = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_EMPTY);

    }

    [Fact]
    public void Error_Price_Less_Than_Zero()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.Price = -5;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRICE_GREATER_THAN_0);

    }

    [Fact]
    public void Error_Initial_Time_Empty()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.InitialTime = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.INITIAL_TIME_EMPTY);

    }

    [Fact]
    public void Error_Initial_Time_Less_Than_Now()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.InitialTime = DateTime.UtcNow.AddMinutes(-5);

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.INITIAL_TIME_LESS_THAN_NOW);

    }

    [Fact]
    public void Error_Final_Time_Empty()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.FinalTime = null;

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.FINAL_TIME_EMPTY);

    }

    [Fact]
    public void Error_Final_Time_Less_Than_Now()
    {
        var validator = new CreatePromotionPriceValidator();

        var request = RequestPromotionPriceBuilder.Build();
        request.FinalTime = request.InitialTime.Value.AddMinutes(-10);

        var result = validator.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.FINAL_TIME_LESS_THAN_INITIAL);

    }
}
