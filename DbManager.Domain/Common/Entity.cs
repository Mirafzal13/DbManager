using System.ComponentModel.DataAnnotations;

namespace DbManager.Domain.Common
{
    public abstract class Entity : IEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
