using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IOrder.Exceptions.ExceptionBase;

public abstract class IOrderException : Exception
{
    public IOrderException(string message) : base(message) { }

    public abstract HttpStatusCode GetStatusCode();

    public abstract List<string> GetErrorMessages();
}
