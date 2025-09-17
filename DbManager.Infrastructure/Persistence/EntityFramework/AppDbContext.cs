using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DbManager.Application.Common;
using DbManager.Application.Common.Abstractions;
using DbManager.Domain.Common;
using DbManager.Domain.Models;
using DbManager.Infrastructure.Persistence.EntityFramework.Extensions;
using System.Reflection;

namespace DbManager.Infrastructure.Persistence.EntityFramework
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : IdentityDbContext<User, Role, Guid>(options), IAppDbContext
    {
        private readonly ICurrentUser _currentUser = currentUser;

        public DbSet<ConnectionConfig> ConnectionConfigs { get; set; }
        public DbSet<SystemEventLog> SystemEventLogs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.SetSoftDeleteFilter();
        }

        public override int SaveChanges()
        {
            SetAuditableEntity();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SetAuditableEntity();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditableEntity()
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity.CreatedById.Equals(Guid.Empty))
                    {
                        entry.Entity.CreatedById = _currentUser.UserId;
                        entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                        entry.Entity.UpdatedById = _currentUser.UserId;
                        entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedById = _currentUser.UserId;
                    entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}
