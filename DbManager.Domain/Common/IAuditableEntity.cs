namespace DbManager.Domain.Common;

public interface IAuditableEntity
{
    Guid CreatedById { get; set; }
    Guid UpdatedById { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
