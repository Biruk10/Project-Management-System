using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

public static class PermissionSeeder
{
    public static readonly string[] TenantPermissions =
    {
        "Project.Create", "Project.Read", "Project.Update", "Project.Delete",
        "Task.Create", "Task.Read", "Task.Update", "Task.Delete", "Task.Assign",
        "Budget.Create", "Budget.Read", "Budget.Approve",
        "Expense.Create", "Expense.Read",
        "Report.View", "Report.Export",
        "User.Create", "User.Read", "User.Update", "User.Delete",
        "Role.Create", "Role.Read", "Role.Update", "Role.Delete",
        "Organization.Read", "Organization.Update",
        "AuditLog.Read", "Notification.Read"
    };

    public static readonly string[] PlatformPermissions =
    {
        "Platform.Organization.View",
        "Platform.Organization.Activate",
        "Platform.Organization.Suspend",
        "Platform.User.View",
        "Platform.User.Activate",
        "Platform.User.Deactivate",
        "Platform.Audit.View",
        "Platform.Report.View",
        "Platform.Settings.View",
        "Platform.Settings.Update",
        "Platform.Support.Access"
    };

    public static readonly string[] AllPermissions =
        TenantPermissions.Concat(PlatformPermissions).ToArray();

    public static async Task SeedAsync(AppDbContext context)
    {
        var existingKeys = await context.Permissions
            .IgnoreQueryFilters()
            .Select(p => p.Key)
            .ToListAsync();

        var toAdd = AllPermissions
            .Where(key => !existingKeys.Contains(key))
            .Select(key => new Permission { Key = key, Description = GetDescription(key) })
            .ToList();

        if (toAdd.Count > 0)
        {
            context.Permissions.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    private static string GetDescription(string key) => key switch
    {
        "Project.Create"  => "Create new projects",
        "Project.Read"    => "View projects",
        "Project.Update"  => "Update project details",
        "Project.Delete"  => "Delete projects",
        "Task.Create"     => "Create tasks",
        "Task.Read"       => "View tasks",
        "Task.Update"     => "Update task details",
        "Task.Delete"     => "Delete tasks",
        "Task.Assign"     => "Assign tasks to users",
        "Budget.Create"   => "Create budgets",
        "Budget.Read"     => "View budgets",
        "Budget.Approve"  => "Approve or reject budgets",
        "Expense.Create"  => "Record expenses",
        "Expense.Read"    => "View expenses",
        "Report.View"     => "View reports",
        "Report.Export"   => "Export reports to PDF or Excel",
        "User.Create"     => "Create users",
        "User.Read"       => "View users",
        "User.Update"     => "Update user details",
        "User.Delete"     => "Delete users",
        "Role.Create"     => "Create roles",
        "Role.Read"       => "View roles",
        "Role.Update"     => "Update roles and permissions",
        "Role.Delete"     => "Delete roles",
        "Organization.Read"   => "View organization details",
        "Organization.Update" => "Update organization settings",
        "AuditLog.Read"       => "View audit logs",
        "Notification.Read"   => "View notifications",
        "Platform.Organization.View"     => "View all platform organizations",
        "Platform.Organization.Activate" => "Activate a suspended organization",
        "Platform.Organization.Suspend"  => "Suspend an organization",
        "Platform.User.View"             => "View all platform users",
        "Platform.User.Activate"         => "Activate a platform user",
        "Platform.User.Deactivate"       => "Deactivate a platform user",
        "Platform.Audit.View"            => "View platform-wide audit logs",
        "Platform.Report.View"           => "View platform-wide reports",
        "Platform.Settings.View"         => "View platform settings",
        "Platform.Settings.Update"       => "Update platform settings",
        "Platform.Support.Access"        => "Access platform support tools",
        _ => key
    };
}
