namespace DbManager.Application.Common.Notifications
{
    public record SystemEventLogNotification(
        Guid UserId,
        EventType EventType,
        string EventData
    ) : INotification;

    public class SystemEventLogHandler(IAppDbContext dbContext) : INotificationHandler<SystemEventLogNotification>
    {
        public async Task Handle(SystemEventLogNotification notification, CancellationToken cancellationToken)
        {
            var log = new SystemEventLog
            {
                UserId = notification.UserId,
                EventType = notification.EventType,
                EventData = notification.EventData,
                DetectedAt = DateTimeOffset.UtcNow
            };

            dbContext.SystemEventLogs.Add(log);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
