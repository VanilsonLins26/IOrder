using CommomTestUtilities.Requests.Cart;
using IOrder.Application.UseCases.Cart.Commands;
using IOrder.Exceptions;
using Shouldly;
using System;
using Xunit;

namespace Validators.Tests.Cart;

public class ChangeQuantityValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new ChangeQuantityValidator();
        var request = ChangeCartItemQuantityRequestBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Empty_CartItemId()
    {
        // Arrange
        var validator = new ChangeQuantityValidator();
        var request = ChangeCartItemQuantityRequestBuilder.Build();
        request.CartItemId = Guid.Empty;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.CART_ITEM_ID_EMPTY);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Error_Invalid_Quantity(int invalidQuantity)
    {
        // Arrange
        var validator = new ChangeQuantityValidator();
        var request = ChangeCartItemQuantityRequestBuilder.Build();
        request.NewQuantity = invalidQuantity;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceMessagesException.QUANTITY_INVALID);
    }
}
