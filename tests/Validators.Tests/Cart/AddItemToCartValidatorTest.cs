using CommomTestUtilities.Requests.Cart;
using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Exceptions;
using Shouldly;
using System;
using Xunit;

namespace Validators.Tests.Cart;

public class AddItemToCartValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new AddItemToCartValidator();
        var request = AddItemToCartRequestBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Empty_ProductId()
    {
        // Arrange
        var validator = new AddItemToCartValidator();
        var request = AddItemToCartRequestBuilder.Build();
        request.ProductId = Guid.Empty;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.PRODUCT_ID_EMPTY);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Error_Invalid_Quantity(int invalidQuantity)
    {
        // Arrange
        var validator = new AddItemToCartValidator();
        var request = AddItemToCartRequestBuilder.Build();
        request.Quantity = invalidQuantity;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.QUANTITY_INVALID);
    }

    [Fact]
    public void Error_Customize_Too_Long()
    {
        // Arrange
        var validator = new AddItemToCartValidator();
        var request = AddItemToCartRequestBuilder.Build();
        request.Customize = new string('A', 501);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CUSTOMIZE_TOO_LONG);
    }
}
