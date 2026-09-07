namespace UNIOOP.App.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string method = context.Request.Method;
            string path = context.Request.Path;

            DateTime startTime = DateTime.UtcNow;

            _logger.LogInformation("Request started. " +
                "Method: {Method}, " +
                "Path: {Path}",
                method,
                path);

            await _next(context);

            DateTime endTime = DateTime.UtcNow;

            TimeSpan duration = endTime - startTime;

            int statusCode = context.Response.StatusCode;

            _logger.LogInformation("Request completed. " +
                "Method: {Method}, " +
                "Path: {Path}, " +
                "StatusCode: {StatusCode}, " +
                "Duration: {Duration} ms",
                method,
                path,
                statusCode,
                duration.TotalMilliseconds);
        }
    }
}