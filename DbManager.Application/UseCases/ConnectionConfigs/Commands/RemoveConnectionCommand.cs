using DbManager.Application.Services;

namespace DbManager.Application.UseCases.ConnectionConfigs.Commands
{
    public record RemoveConnectionCommand(Guid ConnectionId) : IRequest;

    internal sealed class RemoveConnectionCommandHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<RemoveConnectionCommand>
    {
        public async Task Handle(RemoveConnectionCommand command, CancellationToken cancellationToken)
        {
            var connection = activeConnectionService.GetConnection(command.ConnectionId);
            if (connection == null)
                throw new NotFoundException(nameof(ConnectionConfig), command.ConnectionId);

            activeConnectionService.RemoveConnection(command.ConnectionId, connection);
        }
    }
}
