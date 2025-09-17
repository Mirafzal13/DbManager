using DbManager.Application.Common;
using DbManager.Application.Services;
using DbManager.Domain.Models;
using Npgsql;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DbManager.Application.UseCases.ConnectionConfigs.Commands
{
    public record ConnectToServerCommand(string Name,
        string Host,
        int Port,
        string Username,
        string? Password,
        string? EncryptedPasword,
        bool AutoSave = false) : IRequest<Guid>;

    internal sealed class ConnectToServerCommandHandler(IActiveConnectionService activeConnectionService, IPasswordProtector passwordProtector, IAppDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<ConnectToServerCommand, Guid>
    {
        public async Task<Guid> Handle(ConnectToServerCommand command, CancellationToken cancellationToken)
        {
            var encryptedPassword = string.IsNullOrWhiteSpace(command.EncryptedPasword) ? passwordProtector.Protect(command.Password) : command.EncryptedPasword;

            var isConnectionSuccessful = await IsConnectionSuccessful(command, encryptedPassword, cancellationToken);
            if (!isConnectionSuccessful)
                throw new BadRequestException("Unable to establish connection with the provided server credentials. " +
                "Please verify host, port, username, and password.");

            var connectionId = activeConnectionService.AddConnection(new ConnectionConfig
            {
                Name = command.Name,
                Host = command.Host,
                Port = command.Port,
                Username = command.Username,
                EncryptedPassword = encryptedPassword,
            });

            if (command.AutoSave)
            {
                var isExist = await dbContext.ConnectionConfigs.AnyAsync(x => x.Host == command.Host && x.Port == command.Port, cancellationToken);
                if (!isExist)
                {
                    await dbContext.ConnectionConfigs.AddAsync(new ConnectionConfig
                    {
                        Name = command.Name,
                        Host = command.Host,
                        Port = command.Port,
                        Username = command.Username,
                        EncryptedPassword = encryptedPassword,
                        UserId = currentUser.UserId
                    }, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            return connectionId;
        }

        private async Task<bool> IsConnectionSuccessful(ConnectToServerCommand command, string encryptedPassword, CancellationToken cancellationToken)
        {
            var cs = new NpgsqlConnectionStringBuilder
            {
                Host = command.Host,
                Port = command.Port,
                Username = command.Username,
                Password = encryptedPassword,
                Database = "postgres",
                SslMode = SslMode.Disable,
                Timeout = 10
            };

            await using var connection = new NpgsqlConnection(cs.ConnectionString);

            bool isSuccessConnect = false;
            try
            {
                await connection.OpenAsync(cancellationToken);

                isSuccessConnect = true;
            }
            catch (Exception ex)
            {
                isSuccessConnect = false;
            }

            return isSuccessConnect;
        }
    }
}
