using CommomTestUtilities.Requests.Review;
using IOrder.Application.UseCases.Review.Commands;
using Shouldly;

namespace Validators.Tests.Review;

public class CreateReviewValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new CreateReviewValidator();
        var request = CreateReviewRequestBuilder.Build();
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Error_StoreRating_Invalid(int rating)
    {
        var validator = new CreateReviewValidator();
        var request = CreateReviewRequestBuilder.Build();
        request.StoreRating = rating;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "A nota da loja deve ser entre 1 e 5.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Error_CourierRating_Invalid(int rating)
    {
        var validator = new CreateReviewValidator();
        var request = CreateReviewRequestBuilder.Build();
        request.CourierRating = rating;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "A nota do entregador deve ser entre 1 e 5.");
    }
}
