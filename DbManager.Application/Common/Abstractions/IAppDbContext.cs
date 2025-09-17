using DbManager.Domain.Models;

namespace DbManager.Application.Common.Abstractions;

public interface IAppDbContext
{
    DbSet<ConnectionConfig> ConnectionConfigs { get; set; }
    DbSet<SystemEventLog> SystemEventLogs { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
