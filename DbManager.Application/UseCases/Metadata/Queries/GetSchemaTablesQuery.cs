using DbManager.Application.Services;
using Npgsql;

namespace DbManager.Application.UseCases.Metadata.Queries
{
    public record GetSchemaTablesQuery(Guid ConnectionId, string Database, string Schema) : IRequest<List<string>>;

    internal sealed class GetSchemaTablesQueryHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<GetSchemaTablesQuery, List<string>>
    {
        public async Task<List<string>> Handle(GetSchemaTablesQuery request, CancellationToken cancellationToken)
        {
            await using var connection = activeConnectionService.GetConnection(request.ConnectionId, request.Database)
                ?? throw new NotFoundException(nameof(ConnectionConfig), request.ConnectionId);

            await connection.OpenAsync(cancellationToken);

            var result = new List<string>();
            await using var cmd = new NpgsqlCommand(
                "SELECT table_name FROM information_schema.tables WHERE table_schema = @schema;",
                connection);
            cmd.Parameters.AddWithValue("schema", request.Schema);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }
    }
}
