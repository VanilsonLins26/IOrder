using CommomTestUtilities.Requests;
using IOrder.Application.UseCases.Delivery.Commands;
using Shouldly;

namespace Validators.Tests.Delivery;

public class AssignCourierValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new AssignCourierValidator();
        var request = AssignCourierRequestBuilder.Build();
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_CourierUserId_Empty()
    {
        var validator = new AssignCourierValidator();
        var request = AssignCourierRequestBuilder.Build();
        request.CourierUserId = string.Empty;
        
        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage == "ID do entregador é obrigatório.");
    }
}
