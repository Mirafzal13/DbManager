namespace DbManager.Infrastructure.Persistence.EntityFramework.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DbManager.Domain.Models;

public class ConnectionConfigConfiguration : IEntityTypeConfiguration<ConnectionConfig>
{
    public void Configure(EntityTypeBuilder<ConnectionConfig> builder)
    {
        builder.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();
        builder.HasIndex(x => x.Host)
             .IsUnique();
        builder.Property(a => a.Host)
            .HasMaxLength(256);
        builder.Property(a => a.Username)
            .HasMaxLength(64)
            .IsRequired();
        builder.Property(a => a.EncryptedPassword)
           .IsRequired();
    }
}
