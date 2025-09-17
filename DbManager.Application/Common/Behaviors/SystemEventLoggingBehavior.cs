using DbManager.Application.Common.Notifications;

namespace DbManager.Application.Common.Behaviors
{
    public class SystemEventLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IPublisher _publisher;
        private readonly ICurrentUser _currentUser;

        public SystemEventLoggingBehavior(IPublisher publisher, ICurrentUser currentUser)
        {
            _publisher = publisher;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var requestName = typeof(TRequest).Name;

            try
            {
                var response = await next();

                await _publisher.Publish(new SystemEventLogNotification(
                    userId,
                    EventType.Success,
                    $"Request {requestName} executed successfully"
                ), cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                await _publisher.Publish(new SystemEventLogNotification(
                    userId,
                    EventType.Error,
                    $"Request {requestName} failed: {ex.Message}"
                ), cancellationToken);

                throw;
            }
        }
    }
}
