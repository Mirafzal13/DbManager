using DbManager.Application.Common;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace DbManager.Application.Services;

public class ActiveConnectionService : IActiveConnectionService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPasswordProtector _protector;
    private readonly ICurrentUser _currentUser;
    public ActiveConnectionService(IMemoryCache memoryCache, IPasswordProtector protector, ICurrentUser currentUser)
    {
        _memoryCache = memoryCache;
        _protector = protector;
        _currentUser = currentUser;
    }
    public Guid AddConnection(ConnectionConfig connectionConfig)
    {
        var connectionId = Guid.NewGuid();

        _memoryCache.Set(connectionId, connectionConfig, TimeSpan.FromHours(3));

        //active connection larni yig'ish(UserId bilan ajratiladi)
        var allKeys = _memoryCache.GetOrCreate(_currentUser.UserId, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromHours(3);
            return new List<Guid>();
        });

        allKeys.Add(connectionId);
        _memoryCache.Set(_currentUser.UserId, allKeys);

        return connectionId;
    }

    public NpgsqlConnection? GetConnection(Guid connectionId, string? databaseOverride = null)
    {
        if (!_memoryCache.TryGetValue(connectionId, out ConnectionConfig? connectionConfig))
            throw new NotFoundException(nameof(ConnectionConfig), connectionId);

        if (connectionConfig == null)
            throw new NotFoundException(nameof(ConnectionConfig), connectionId);

        var password = _protector.Unprotect(connectionConfig.EncryptedPassword);

        var cs = new NpgsqlConnectionStringBuilder
        {
            Host = connectionConfig.Host,
            Port = connectionConfig.Port,
            Username = connectionConfig.Username,
            Password = password,
            Database = databaseOverride ?? connectionConfig.Database ?? "postgres", //birinchi marta server ga ulanish uchun. Keyinchalik database aniq ko'rsatiladi
            SslMode = SslMode.Disable,
            Timeout = 10
        };

        //connection faqat method chaqirilgan joyda ochiladi
        var connection = new NpgsqlConnection(cs.ConnectionString);
        return connection;
    }

    public void RemoveConnection(Guid connectionId, NpgsqlConnection connection)
    {
        //connection config ni cache dan tozalash
        if (!_memoryCache.TryGetValue(connectionId, out ConnectionConfig? connectionConfig))
            throw new NotFoundException(nameof(ConnectionConfig), connectionId);

        //connection config ni active connection lardan olib tashlash
        if (_memoryCache.TryGetValue(_currentUser.UserId, out List<Guid>? allKeys))
        {
            allKeys?.Remove(connectionId);
            _memoryCache.Set(_currentUser.UserId, allKeys);
        }

        _memoryCache.Remove(connectionId);

        //database connection ni dispose qilish(close ichkarida o'zi chaqiradi va qolgan keraksiz ma'lumotlarni ham o'chiradi)
        connection.Dispose();
    }

    public IReadOnlyDictionary<Guid, ConnectionConfig> GetActiveConnections()
    {
        var result = new Dictionary<Guid, ConnectionConfig>();

        //UserId yordamida active connetion larni olish
        if (_memoryCache.TryGetValue(_currentUser.UserId, out List<Guid>? keys))
        {
            foreach (var key in keys)
            {
                if (_memoryCache.TryGetValue(key, out ConnectionConfig? config) && config != null)
                {
                    result[key] = config;
                }
            }
        }

        return result;
    }
}