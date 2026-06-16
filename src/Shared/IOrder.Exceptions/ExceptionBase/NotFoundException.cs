using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IOrder.Exceptions.ExceptionBase;

public class NotFoundException : IOrderException
{
    private readonly List<string> _errors;

    public NotFoundException(List<string> errors) : base(string.Empty)
    {
        _errors = errors;
    }

    public override List<string> GetErrorMessages() => _errors;

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
}
