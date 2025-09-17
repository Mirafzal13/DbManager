namespace DbManager.Domain.Common;

/// <summary>
/// Базовый класс для сущностей поддерживающих аудит 
/// </summary>
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    /// <summary>
    /// Запись создан в 
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Запись обновлен в
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Запись создан кем
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Запись обновлен кем
    /// </summary>
    public Guid UpdatedById { get; set; }
}
