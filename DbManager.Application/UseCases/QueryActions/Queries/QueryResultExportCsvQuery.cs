using DbManager.Application.UseCases.QueryActions.Commands;
using System.Text;

namespace DbManager.Application.UseCases.QueryActions.Queries
{
    public record QueryResultExportCsvQuery(Guid ConnectionId, string Database, string Sql) : IRequest<byte[]>;

    internal sealed class QueryResultExportCsvQueryHandler(ISender sender) : IRequestHandler<QueryResultExportCsvQuery, byte[]>
    {
        public async Task<byte[]> Handle(QueryResultExportCsvQuery request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new QueryExecuteCommand(request.ConnectionId, request.Database, request.Sql));

            if (!result.IsSelect)
            {
                throw new BadRequestException("CSV export is only supported for SELECT queries.");
            }

            var sb = new StringBuilder();

            sb.AppendLine(string.Join(",", result.Columns));

            foreach (var row in result.Rows)
            {
                var values = row.Select(v => v?.ToString()?.Replace(",", ";") ?? "");
                sb.AppendLine(string.Join(",", values));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
