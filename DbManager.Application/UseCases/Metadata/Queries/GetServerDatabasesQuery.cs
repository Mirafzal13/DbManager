using DbManager.Application.Services;
using Npgsql;

namespace DbManager.Application.UseCases.Metadata.Queries
{
    public record GetServerDatabasesQuery(Guid ConnectionId) : IRequest<List<string>>;

    internal sealed class GetServerDatabasesQueryHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<GetServerDatabasesQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetServerDatabasesQuery request, CancellationToken cancellationToken)
        {
            await using var connection = activeConnectionService.GetConnection(request.ConnectionId, "postgres")
                ?? throw new AccessDeniedException();

            await connection.OpenAsync(cancellationToken);

            var result = new List<string>();

            var cmd = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false;", connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }
    }
}
