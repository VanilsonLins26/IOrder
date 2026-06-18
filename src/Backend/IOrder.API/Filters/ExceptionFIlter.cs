using IOrder.Communication.Response;
using IOrder.Exceptions;
using IOrder.Exceptions.ExceptionBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOrder.API.Filters;

public class ExceptionFIlter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is IOrderException)
            HandleProjectException(context);

        else
            ThrowUnknowException(context);
    }

    public static void HandleProjectException(ExceptionContext context)
    {
        if(context.Exception is IOrderException iOrderException)
        {
            context.HttpContext.Response.StatusCode = (int)iOrderException.GetStatusCode();
            context.Result = new ObjectResult(new ResponseErrorDto(iOrderException.GetErrorMessages()));
        }

    }

    private static void ThrowUnknowException(ExceptionContext context)
    {

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorDto(ResourceMessagesException.UNKNOWN_ERROR));

    }
}

