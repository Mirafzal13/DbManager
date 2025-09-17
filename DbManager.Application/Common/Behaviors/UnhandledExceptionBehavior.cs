namespace DbManager.Application.Common.Behaviors;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

public class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger = logger;
    private static readonly Action<ILogger, string, string, Exception> RequestFailed
        = LoggerMessage.Define<string, string>(LogLevel.Error, new EventId(2, typeof(TRequest).Name), "Request: Unhandled Exception for Request {Name} ({@Request})");

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            RequestFailed(_logger, typeof(TRequest).Name, request.ToString()!, ex);

            throw;
        }
    }
}
