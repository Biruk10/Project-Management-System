using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

public static class SystemAdminSeeder
{
    public const string SystemOrgName   = "System";
    public const string SystemAdminRole = "SystemAdmin";
    public const string SystemAdminEmail = "admin@system.local";
    public const string DefaultPassword  = "System@Admin123!";

    private static readonly PasswordHasher<User> Hasher = new();

    public static async Task SeedAsync(AppDbContext context)
    {
        var systemOrg = await context.Organizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Name == SystemOrgName);

        if (systemOrg == null)
        {
            systemOrg = new Organization
            {
                Name = SystemOrgName, Email = "system@platform.local",
                TimeZone = "UTC", IsActive = true, CreatedAt = DateTime.UtcNow
            };
            context.Organizations.Add(systemOrg);
            await context.SaveChangesAsync();
        }

        var role = await context.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.OrganizationId == systemOrg.Id && r.Name == SystemAdminRole);

        if (role == null)
        {
            role = new Role
            {
                OrganizationId = systemOrg.Id,
                Name = SystemAdminRole,
                Description = "Platform-scoped SaaS administrator. Manages organisations, platform users, security and support. Does NOT access individual organisation business data.",
                IsSystemRole = true
            };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }

        var user = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == SystemAdminEmail);

        if (user == null)
        {
            var dummy = new User();
            user = new User
            {
                OrganizationId = systemOrg.Id,
                FirstName = "System", LastName = "Administrator",
                Email = SystemAdminEmail,
                PasswordHash = Hasher.HashPassword(dummy, DefaultPassword),
                IsActive = true, Status = UserStatus.Active, CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.UserRoles.Add(new UserRole
            {
                OrganizationId = systemOrg.Id, UserId = user.Id,
                RoleId = role.Id, AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
        else
        {
            var verifyResult = Hasher.VerifyHashedPassword(user, user.PasswordHash, DefaultPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                user.PasswordHash = Hasher.HashPassword(user, DefaultPassword);
                await context.SaveChangesAsync();
            }
        }

        await AssignPlatformPermissionsAsync(context, role.Id);
    }

    private static async Task AssignPlatformPermissionsAsync(AppDbContext context, int roleId)
    {
        var platformIds = await context.Permissions
            .IgnoreQueryFilters()
            .Where(p => p.Key.StartsWith("Platform."))
            .Select(p => p.Id)
            .ToListAsync();

        var existing = await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var missing = platformIds.Except(existing).ToList();
        if (missing.Count > 0)
        {
            context.RolePermissions.AddRange(
                missing.Select(id => new RolePermission { RoleId = roleId, PermissionId = id })
            );
            await context.SaveChangesAsync();
        }
    }
}
