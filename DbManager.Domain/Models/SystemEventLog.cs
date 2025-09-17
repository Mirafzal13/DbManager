using DbManager.Domain.Common;
using DbManager.Domain.Enums;

namespace DbManager.Domain.Models;

public class SystemEventLog : Entity
{
    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }

    /// <summary>
    /// EventType
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// EventData
    /// </summary>
    public string? EventData { get; set; }

    /// <summary>
    /// DetectedAt
    /// </summary>
    public DateTimeOffset DetectedAt { get; set; }
}
