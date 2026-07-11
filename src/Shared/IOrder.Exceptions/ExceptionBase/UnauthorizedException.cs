using System.Net;

namespace IOrder.Exceptions.ExceptionBase;

public class UnauthorizedException : IOrderException
{
    private readonly List<string> _errors;

    public UnauthorizedException(string message) : base(string.Empty)
    {
        _errors = [message];
    }

    public override List<string> GetErrorMessages() => _errors;

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
