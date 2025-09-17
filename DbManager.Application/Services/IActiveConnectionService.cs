using Npgsql;

namespace DbManager.Application.Services;

public interface IActiveConnectionService
{
    Guid AddConnection(ConnectionConfig connectionConfig);
    NpgsqlConnection? GetConnection(Guid connectionId, string? databaseOverride = null);
    void RemoveConnection(Guid connectionId, NpgsqlConnection connection);
    IReadOnlyDictionary<Guid, ConnectionConfig> GetActiveConnections();
}