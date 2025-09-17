using DbManager.Application.Services;
using Npgsql;

namespace DbManager.Application.UseCases.Metadata.Queries
{
    public record GetDatabaseSchemasQuery(Guid ConnectionId, string Database) : IRequest<List<string>>;

    internal sealed class GetDatabaseSchemasQueryHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<GetDatabaseSchemasQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetDatabaseSchemasQuery request, CancellationToken cancellationToken)
        {
            await using var connection = activeConnectionService.GetConnection(request.ConnectionId, request.Database);
            if (connection == null)
                throw new NotFoundException(nameof(ConnectionConfig), request.ConnectionId);

            await connection.OpenAsync(cancellationToken);

            var result = new List<string>();
            await using var cmd = new NpgsqlCommand("SELECT schema_name FROM information_schema.schemata;", connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }
    }
}
