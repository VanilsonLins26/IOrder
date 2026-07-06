using IOrder.Exceptions.ExceptionBase;
using Shouldly;
using System.Net;

namespace UseCases.Test.Exceptions;

public class UnauthorizedStoreExceptionTest
{
    [Fact]
    public void Should_Have_Forbidden_StatusCode()
    {
        var exception = new UnauthorizedStoreException(["Ação não autorizada."]);

        var statusCode = exception.GetStatusCode();

        statusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public void Should_Return_Error_Messages()
    {
        var errors = new List<string> { "Erro 1", "Erro 2" };
        var exception = new UnauthorizedStoreException(errors);

        var result = exception.GetErrorMessages();

        result.ShouldBe(errors);
    }

    [Fact]
    public void Should_Return_Single_Error()
    {
        var exception = new UnauthorizedStoreException(["Apenas um erro."]);

        var result = exception.GetErrorMessages();

        result.ShouldHaveSingleItem().ShouldBe("Apenas um erro.");
    }
}
