using DbManager.Application.Services;

namespace DbManager.Application.UseCases.ConnectionConfigs.Commands
{
    public record TestConnectionCommand(Guid ConnectionId) : IRequest<bool>;

    internal sealed class TestConnectionCommandHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<TestConnectionCommand, bool>
    {
        public async Task<bool> Handle(TestConnectionCommand command, CancellationToken cancellationToken)
        {
            bool result = false;
            try
            {
                await using var connection = activeConnectionService.GetConnection(command.ConnectionId);

                if (connection != null)
                {
                    await connection.OpenAsync(cancellationToken);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}
