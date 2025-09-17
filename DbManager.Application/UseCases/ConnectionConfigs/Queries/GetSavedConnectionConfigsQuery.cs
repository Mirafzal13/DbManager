using DbManager.Application.Common;
using DbManager.Application.UseCases.ConnectionConfigs.Models;

namespace DbManager.Application.UseCases.ConnectionConfigs.Queries
{
    public record GetSavedConnectionConfigsQuery : IRequest<PagedList<ConnectionConfigModel>>;

    internal sealed class GetSavedConnectionConfigsQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser, IMapper mapper) : IRequestHandler<GetSavedConnectionConfigsQuery, PagedList<ConnectionConfigModel>>
    {
        public async Task<PagedList<ConnectionConfigModel>> Handle(GetSavedConnectionConfigsQuery request, CancellationToken cancellationToken)
        {
            var connectionConfigs = dbContext.ConnectionConfigs.Where(x => x.UserId == currentUser.UserId).AsQueryable();
            var count = await connectionConfigs.CountAsync(cancellationToken);

            var result = await connectionConfigs.ProjectTo<ConnectionConfigModel>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            return new PagedList<ConnectionConfigModel>(result, count);
        }
    }
}
