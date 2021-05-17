namespace Sibedge.Plv8Demo.WebApi.Filters
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary> Exception Filter Attribute </summary>
    public sealed class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary> On exception </summary>
        /// <param name="context"> Context </param>
        public override void OnException(ExceptionContext context)
        {
            int statusCode;

            if (context.Exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
            }

            context.HttpContext.Response.StatusCode = statusCode;

            context.Result = new JsonResult(new
                             {
                                 title = context.Exception.Message,
                                 status = statusCode,
                                 exception = context.Exception.GetType(),
                             });
        }
    }
}
