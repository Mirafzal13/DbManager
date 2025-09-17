using Microsoft.AspNetCore.Identity;
using DbManager.Domain.Models;
using DbManager.Infrastructure.Persistence.EntityFramework;

namespace DbManager.Infrastructure.Extensions.DataSeeding
{
    public static class SeedDataExtensions
    {
        public static void Seed(this AppDbContext context)
        {
            if (context == null)
                return;

            if (!context.Users.Any())
            {
                var passwordHasher = new PasswordHasher<User>();
                var adminUser = new User
                {
                    Id = Guid.Parse("d392188c-15ee-4a5c-8ef5-9b7e810e3ea9"),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    UserName = "admin",
                    Email = "admin@info.uz",
                    NormalizedUserName = "ADMIN",
                    NormalizedEmail = "ADMIN@INFO.UZ",
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "123456Test$");

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            if (!context.Roles.Any())
            {
                var superAdminRole = new Role
                {
                    Id = Guid.Parse("38a4bd2e-a205-46ff-bc5c-04f0c4e6a70b"),
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                };
                context.Roles.Add(superAdminRole);
                context.SaveChanges();
            }

            if (!context.UserRoles.Any())
            {
                context.UserRoles.Add(new IdentityUserRole<Guid>
                {
                    RoleId = Guid.Parse("38a4bd2e-a205-46ff-bc5c-04f0c4e6a70b"),
                    UserId = Guid.Parse("d392188c-15ee-4a5c-8ef5-9b7e810e3ea9")
                });
                context.SaveChanges();
            }
        }
    }
}
