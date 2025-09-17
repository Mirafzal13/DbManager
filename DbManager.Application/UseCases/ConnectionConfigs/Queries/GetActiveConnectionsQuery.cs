using DbManager.Application.Services;
using DbManager.Application.UseCases.ConnectionConfigs.Models;

namespace DbManager.Application.UseCases.ConnectionConfigs.Queries
{
    public record GetActiveConnectionsQuery() : IRequest<PagedList<ActiveConnectionModel>>;

    internal sealed class GetActiveConnectionsQueryHandler(IActiveConnectionService activeConnectionService) : IRequestHandler<GetActiveConnectionsQuery, PagedList<ActiveConnectionModel>>
    {
        public async Task<PagedList<ActiveConnectionModel>> Handle(GetActiveConnectionsQuery query, CancellationToken cancellationToken)
        {
            var activeConnections = activeConnectionService.GetActiveConnections();

            var result = new List<ActiveConnectionModel>();
            foreach (var connection in activeConnections)
            {
                var connectionId = connection.Key;
                var connectionName = connection.Value.Name;

                result.Add(new ActiveConnectionModel { ConnectionId = connectionId, ServerName = connectionName });
            }

            return new PagedList<ActiveConnectionModel>(result, activeConnections.Count);
        }
    }
}
