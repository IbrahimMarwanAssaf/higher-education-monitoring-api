using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UNIOOP.App.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            int statusCode;
            string title;
            string detail;

            if (exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                title = "Not Found";
                detail = exception.Message;
            }
            else if (exception is ConflictException)
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "Conflict";
                detail = exception.Message;
            }
            else if (exception is BadRequestException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                detail = exception.Message;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Internal Server Error";
                detail = "An unexpected error occurred.";
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail
                },
                cancellationToken);

            return true;
        }
    }
}