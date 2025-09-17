using DbManager.Application.UseCases.QueryActions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DbManager.Application.UseCases.QueryActions.Queries
{
    public record GetQueriesHistoryQuery(Guid ConnectionId) : IRequest<List<QueryHistoryModel>>;

    internal sealed class GetQueriesHistoryQueryHandler(IMemoryCache memoryCache) : IRequestHandler<GetQueriesHistoryQuery, List<QueryHistoryModel>>
    {
        public async Task<List<QueryHistoryModel>> Handle(GetQueriesHistoryQuery request, CancellationToken cancellationToken)
        {
            if (memoryCache.TryGetValue($"history:{request.ConnectionId}", out List<QueryHistoryModel>? history))
            {
                return history!;
            }

            return new List<QueryHistoryModel>();
        }
    }
}
