namespace DbManager.Application.Common.Behaviors;

using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

public class LoggingBehavior<TRequest>(ILogger<TRequest> logger)
    : IRequestPreProcessor<TRequest>
{
    private readonly ILogger _logger = logger;
    private static readonly Action<ILogger, string, string, Exception?> RequestLog
        = LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(1, typeof(TRequest).Name), "Starting a request of {Name} ({@Request})");

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        RequestLog(_logger, typeof(TRequest).Name, request?.ToString() ?? string.Empty, null);

        return Task.CompletedTask;
    }
}
