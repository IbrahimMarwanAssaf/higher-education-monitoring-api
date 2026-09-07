using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace UNIOOP.App.Filters
{
    public class ExecutionTimeFilter : IAsyncActionFilter
    {
        private readonly ILogger<ExecutionTimeFilter> _logger;

        public ExecutionTimeFilter(ILogger<ExecutionTimeFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await next();
            stopwatch.Stop();

            string controllerName = context.Controller.GetType().Name;
            string actionName = context.ActionDescriptor.DisplayName ?? "Unknown";

            _logger.LogInformation(
                "{ControllerName}.{ActionName} " +
                "executed in {ElapsedMilliseconds} ms.",
                controllerName,
                actionName,
                stopwatch.ElapsedMilliseconds);
        }
    }
}