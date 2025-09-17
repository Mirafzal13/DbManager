using DbManager.Application.Services;
using DbManager.Application.UseCases.Metadata.Models;
using Npgsql;

namespace DbManager.Application.UseCases.Metadata.Queries
{
    public record GetTableColumnsQuery(Guid ConnectionId,
        string Database,
        string Schema,
        string Table) : IRequest<List<ColumnInfoModel>>;

    internal sealed class GetTableColumnsQueryHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<GetTableColumnsQuery, List<ColumnInfoModel>>
    {
        public async Task<List<ColumnInfoModel>> Handle(GetTableColumnsQuery request, CancellationToken cancellationToken)
        {
            await using var conn = activeConnectionService.GetConnection(request.ConnectionId, request.Database)
                ?? throw new NotFoundException(nameof(ConnectionConfig), request.ConnectionId);

            await conn.OpenAsync(cancellationToken);

            var result = new List<ColumnInfoModel>();
            var sql = @"
            SELECT 
                column_name, 
                data_type, 
                is_nullable,
                character_maximum_length,
                EXISTS (
                    SELECT 1 
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu
                        ON tc.constraint_name = kcu.constraint_name
                        AND tc.table_schema = kcu.table_schema
                    WHERE tc.constraint_type = 'PRIMARY KEY'
                        AND tc.table_name = c.table_name
                        AND kcu.column_name = c.column_name
                ) as is_primary_key
            FROM information_schema.columns c
            WHERE table_schema = @schema AND table_name = @table;
        ";

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("schema", request.Schema);
            cmd.Parameters.AddWithValue("table", request.Table);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new ColumnInfoModel
                {
                    Name = reader.GetString(0),
                    DataType = reader.GetString(1),
                    IsNullable = reader.GetString(2) == "YES",
                    MaxLength = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    IsPrimaryKey = reader.GetBoolean(4)
                });
            }

            return result;
        }
    }
}
