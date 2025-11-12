using Famnances.DataCore.Entities;
using Famnances.Core.Security;
using FamnancesServices.Business.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Famnances.Core.Errors;

namespace FamnancesServices.Helper
{
    public class ErrorHandler(ILogger<ErrorHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            string? token = httpContext.Items[Constants.TOKEN] as string;

            _logger.LogError(exception, "API Exception: {Message}", exception.Message);
            var errorLogManager = httpContext.RequestServices.GetRequiredService<IErrorLogManager>();
            try
            {
                var errorLog = new ErrorLog
                {
                    Timestamp = DateTime.UtcNow,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Path = httpContext.Request.Path,
                    HttpMethod = httpContext.Request.Method,
                    QueryString = httpContext.Request.QueryString.ToString()
                };

                errorLogManager.Add(errorLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could'nt use webservice");
            }

            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "Internal Server Error";
            var detail = "An unexpected error occurred while processing the request.";

            switch (exception)
            {
                case AppException e:
                    statusCode = e.StatusCode;
                    title = e.Message;
                    detail = e.Message;
                    break;
                case KeyNotFoundException e:
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Resource Not Found";
                    detail = "The requested resource could not be found.";
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    title = "Internal Server Error";
                    detail = "An unexpected error occurred while processing the request.";
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}