using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMember> ProjectMembers { get; }
    DbSet<TaskItem> TaskItems { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<BudgetLine> BudgetLines { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

