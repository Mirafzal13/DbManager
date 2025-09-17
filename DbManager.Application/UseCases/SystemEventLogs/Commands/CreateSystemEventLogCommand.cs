namespace DbManager.Application.UseCases.SystemEventLogs.Commands
{
    public record CreateSystemEventLogCommand(
        Guid UserId,
        EventType EventType,
        string? EventData
    ) : IRequest<bool>;

    public class CreateSystemEventLogHandler(IAppDbContext dbContext) : IRequestHandler<CreateSystemEventLogCommand, bool>
    {
        public async Task<bool> Handle(CreateSystemEventLogCommand request, CancellationToken cancellationToken)
        {
            var log = new SystemEventLog
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                EventType = request.EventType,
                EventData = request.EventData,
                DetectedAt = DateTimeOffset.Now
            };

            dbContext.SystemEventLogs.Add(log);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
