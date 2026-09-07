using Microsoft.AspNetCore.Mvc.Filters;

namespace UNIOOP.App.Filters
{
    public class AuditDeleteFilter : IAsyncActionFilter
    {
        private readonly ILogger<AuditDeleteFilter> _logger;

        public AuditDeleteFilter(ILogger<AuditDeleteFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string controllerName = context.Controller.GetType().Name;
            string actionName = context.ActionDescriptor.DisplayName ?? "Unknown";
            string httpMethod = context.HttpContext.Request.Method;
            string path = context.HttpContext.Request.Path;

            _logger.LogInformation(
                "Delete request started. " +
                "Controller: {ControllerName}, " +
                "Action: {ActionName}, " +
                "Method: {HttpMethod}, " +
                "Path: {Path}",
                controllerName,
                actionName,
                httpMethod,
                path);

            ActionExecutedContext executedContext = await next();

            int statusCode = context.HttpContext.Response.StatusCode;

            if (executedContext.Exception is null || executedContext.ExceptionHandled)
            {
                _logger.LogInformation(
                    "Delete request completed. " +
                    "Controller: {ControllerName}, " +
                    "Action: {ActionName}",
                    controllerName,
                    actionName);
            }
            else
            {
                _logger.LogWarning(
                    "Delete request failed. " +
                    "Controller: {ControllerName}, " +
                    "Action: {ActionName}",
                    controllerName,
                    actionName);
            }
        }
    }
}