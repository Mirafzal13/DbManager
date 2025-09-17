using DbManager.Domain.Common;

namespace DbManager.Domain.Models;

public class ConnectionConfig : AuditableEntity, IDeletable
{
    /// <summary>
    /// Name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Host
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Database
    /// </summary>
    public string? Database { get; set; }

    /// <summary>
    /// Username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// EncryptedPassword
    /// </summary>
    public required string EncryptedPassword { get; set; }

    /// <summary>
    /// IsDeleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
