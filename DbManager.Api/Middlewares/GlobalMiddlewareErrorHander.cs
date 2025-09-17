namespace DbManager.Api.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DbManager.Api.Extensions;

public class GlobalMiddlewareErrorHander(RequestDelegate next, ILogger<GlobalMiddlewareErrorHander> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalMiddlewareErrorHander> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);

            _logger.LogError(ex, "Internal server error");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex.GetStatusCode();
        var problemDetails = ExceptionHandlerExtensions.GetProblemDetailsText(ex, (int)statusCode);

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode;

        await response.WriteAsync(problemDetails);
    }
}
