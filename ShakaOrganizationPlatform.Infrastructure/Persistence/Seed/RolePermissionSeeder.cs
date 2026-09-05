using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

public static class RolePermissionSeeder
{
    // OrganizationAdmin gets all tenant permissions — never Platform.*
    private static readonly string[] AdminPermissions = PermissionSeeder.TenantPermissions;

    private static readonly string[] ProjectManagerPermissions =
    {
        "Project.Create", "Project.Read", "Project.Update",
        "Task.Create", "Task.Read", "Task.Update", "Task.Delete", "Task.Assign",
        "Budget.Read", "Expense.Read",
        "Report.View", "Notification.Read", "User.Read"
    };

    private static readonly string[] FinanceOfficerPermissions =
    {
        "Project.Read",
        "Budget.Create", "Budget.Read", "Budget.Approve",
        "Expense.Create", "Expense.Read",
        "Report.View", "Report.Export", "Notification.Read"
    };

    private static readonly string[] EmployeePermissions =
    {
        "Project.Read", "Task.Read", "Task.Update", "Notification.Read"
    };

    public static async Task SeedRolePermissionsAsync(AppDbContext context, int organizationId, int adminRoleId, int pmRoleId, int financeRoleId, int employeeRoleId)
    {
        var allPermissions = await context.Permissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(p => p.Key, p => p.Id);

        var existingLinks = await context.RolePermissions
            .Where(rp => rp.Role.OrganizationId == organizationId)
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToListAsync();

        var existing = existingLinks
            .Select(x => (x.RoleId, x.PermissionId))
            .ToHashSet();

        var toAdd = new List<RolePermission>();

        AddMissing(toAdd, existing, adminRoleId,   AdminPermissions,          allPermissions);
        AddMissing(toAdd, existing, pmRoleId,      ProjectManagerPermissions, allPermissions);
        AddMissing(toAdd, existing, financeRoleId, FinanceOfficerPermissions, allPermissions);
        AddMissing(toAdd, existing, employeeRoleId, EmployeePermissions,      allPermissions);

        if (toAdd.Count > 0)
        {
            context.RolePermissions.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    public static async Task RepairExistingOrganizationsAsync(AppDbContext context)
    {
        var orgIds = await context.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Name != SystemAdminSeeder.SystemOrgName)
            .Select(o => o.Id)
            .ToListAsync();

        foreach (var orgId in orgIds)
        {
            var roles = await context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.OrganizationId == orgId && r.IsSystemRole)
                .ToListAsync();

            var adminRole   = roles.FirstOrDefault(r => r.Name == "OrganizationAdmin");
            var pmRole      = roles.FirstOrDefault(r => r.Name == "ProjectManager");
            var financeRole = roles.FirstOrDefault(r => r.Name == "FinanceOfficer");
            var employeeRole= roles.FirstOrDefault(r => r.Name == "Employee");

            if (adminRole == null) continue;

            await SeedRolePermissionsAsync(
                context, orgId,
                adminRole.Id,
                pmRole?.Id ?? 0,
                financeRole?.Id ?? 0,
                employeeRole?.Id ?? 0
            );
        }
    }

    private static void AddMissing(
        List<RolePermission> toAdd,
        HashSet<(int RoleId, int PermissionId)> existing,
        int roleId,
        string[] permissionKeys,
        Dictionary<string, int> allPermissions)
    {
        if (roleId == 0) return;
        foreach (var key in permissionKeys)
        {
            if (!allPermissions.TryGetValue(key, out var permId)) continue;
            if (existing.Contains((roleId, permId))) continue;
            toAdd.Add(new RolePermission { RoleId = roleId, PermissionId = permId });
        }
    }
}
