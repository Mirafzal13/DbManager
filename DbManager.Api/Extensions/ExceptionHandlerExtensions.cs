namespace DbManager.Api.Extensions;

using System.Globalization;
using System.Linq;
using System.Net;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DbManager.Domain.Exceptions;

public static class ExceptionHandlerExtensions
{
    private static readonly DefaultContractResolver ContractResolver = new()
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };

    public static IApplicationBuilder UseApplicationExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(config =>
            config.Run(async context =>
                await HandleExceptionAsync(context).ConfigureAwait(false)));

        return app;
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var exception = contextFeature.Error;
            var statusCode = exception.GetStatusCode();
            var problemDetails = GetProblemDetails(exception, (int)statusCode);

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
            {
                ContractResolver = ContractResolver,
                Formatting = Formatting.Indented
            })).ConfigureAwait(false);

            var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ExceptionHandler");
            logger.LogError(exception, problemDetails?.Title);
        }
    }

    public static string GetProblemDetailsText(Exception exception, int statusCode)
    {
        var problemDetails = GetProblemDetails(exception, statusCode);

        return JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
        {
            ContractResolver = ContractResolver,
            Formatting = Formatting.Indented
        });
    }

    public static ProblemDetails GetProblemDetails(Exception exception, int statusCode)
    {
        string? errorMessage = null;

        if (exception is AppException appException && !string.IsNullOrWhiteSpace(appException.MessageFormat))
            errorMessage = string.Format(CultureInfo.InvariantCulture, exception.Message, appException.Args);

        if (string.IsNullOrWhiteSpace(errorMessage))
            errorMessage = exception.Message;

        var (message, errors) = exception switch
        {
            ValidationException => (errorMessage, ((ValidationException)exception)?.Errors?.ToDictionary()),
            _ => (errorMessage, null),
        };

        return errors == null
            ? new ProblemDetails
            {
                Title = message,
                Status = statusCode
            }
            : new HttpValidationProblemDetails(errors)
            {
                Title = message,
                Status = statusCode,
            };
    }

    public static HttpStatusCode GetStatusCode(this Exception exception)
    {
        return exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            AccessDeniedException => HttpStatusCode.Forbidden,
            ApplicationException
            or ValidationException
            or BadRequestException => HttpStatusCode.BadRequest,
            AlreadyExistsException or
            UniqueConstraintException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };
    }

    public static IDictionary<string, string[]> ToDictionary(this IEnumerable<ValidationFailure> errors)
    {
        return errors
          .GroupBy(x => x.PropertyName)
          .ToDictionary(
            g => g.Key,
            g => g.Select(x => x.ErrorMessage).ToArray());
    }
}
