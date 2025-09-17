using DbManager.Application.Services;
using DbManager.Application.UseCases.QueryActions.Models;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace DbManager.Application.UseCases.QueryActions.Commands
{
    public record QueryExecuteCommand(Guid ConnectionId,
        string Database,
        string SqlQuery) : IRequest<ExecuteQueryResultModel>;

    internal sealed class QueryExecuteCommandHandler(IActiveConnectionService activeConnectionService, IMemoryCache memoryCache) : IRequestHandler<QueryExecuteCommand, ExecuteQueryResultModel>
    {
        public async Task<ExecuteQueryResultModel> Handle(QueryExecuteCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SqlQuery))
                throw new BadRequestException("SQL query must not be empty.");

            await using var conn = activeConnectionService.GetConnection(request.ConnectionId, request.Database)
                ?? throw new NotFoundException(nameof(ConnectionConfig), request.ConnectionId);

            await conn.OpenAsync(cancellationToken);

            var result = new ExecuteQueryResultModel();

            try
            {
                await using var cmd = new NpgsqlCommand(request.SqlQuery, conn);

                //query qaysi turdaligini aniqlash(response shunga qarab chiqadi)
                if (request.SqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    //Select query
                    //UI uchun sql query turini bildiruchi field
                    result.IsSelect = true;

                    await using var reader = await cmd.ExecuteReaderAsync();

                    int columnCount = reader.FieldCount;

                    for (int i = 0; i < columnCount; i++)
                    {
                        result.Columns.Add(reader.GetName(i));
                    }

                    while (await reader.ReadAsync())
                    {
                        var row = new List<object?>();
                        for (int i = 0; i < columnCount; i++)
                        {
                            row.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
                        }
                        result.Rows.Add(row);
                    }

                    result.AffectedRows = result.Rows.Count;
                }
                else
                {
                    //CREATE, ALTER, INSERT, UPDATE, DELETE
                    result.IsSelect = false;

                    result.AffectedRows = await cmd.ExecuteNonQueryAsync();
                }

                //sql query ni istoriyasini chache ga saqlash
                SaveHistory(request.ConnectionId, request.Database, request.SqlQuery);

                return result;
            }
            catch (PostgresException pgEx)
            {
                throw new InvalidOperationException($"Postgres error: {pgEx.Message}", pgEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to execute query: {ex.Message}", ex);
            }
        }

        private void SaveHistory(Guid connectionId, string database, string sql)
        {
            var historyKey = $"history:{connectionId}";

            if (!memoryCache.TryGetValue(historyKey, out List<QueryHistoryModel>? history))
            {
                history = new List<QueryHistoryModel>();
            }

            history.Insert(0, new QueryHistoryModel
            {
                Id = Guid.NewGuid(),
                ConnectionId = connectionId,
                Database = database,
                Sql = sql,
                ExecutedAt = DateTime.UtcNow
            });

            // faqat oxirgi 50 query ni saqlidi
            if (history.Count > 50)
                history = history.Take(50).ToList();

            memoryCache.Set(historyKey, history, TimeSpan.FromHours(3));
        }
    }
}
