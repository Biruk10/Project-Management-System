using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Common;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ITenantService? _tenantService;
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantService? tenantService = null,
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public int? CurrentOrganizationId => _tenantService?.OrganizationId;
    public bool HasTenant => _tenantService?.HasTenant ?? false;

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetLine> BudgetLines => Set<BudgetLine>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Tenant Query Filters for all ITenantEntity entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateTenantFilter(entityType.ClrType));
            }
        }

        // Decimal Precision Configuration
        modelBuilder.Entity<Project>()
            .Property(p => p.ProgressPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<TaskItem>()
            .Property(t => t.CompletionPercentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Budget>()
            .Property(b => b.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BudgetLine>()
            .Property(bl => bl.AllocatedAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        // Indexes
        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Key)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.OrganizationId, u.Email })
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => new { r.OrganizationId, r.Name })
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();

        modelBuilder.Entity<ProjectMember>()
            .HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();

        // User Relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Role Relationships
        modelBuilder.Entity<Role>()
            .HasOne(r => r.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole Relationships
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Organization)
            .WithMany()
            .HasForeignKey(ur => ur.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // RolePermission Relationships
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project Relationships
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.Projects)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.ProjectManager)
            .WithMany(u => u.ManagedProjects)
            .HasForeignKey(p => p.ProjectManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // ProjectMember Relationships
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Organization)
            .WithMany()
            .HasForeignKey(pm => pm.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskItem Relationships
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Organization)
            .WithMany()
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Budget Relationships
        modelBuilder.Entity<Budget>()
            .HasOne(b => b.Project)
            .WithMany(p => p.Budgets)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Budget>()
            .HasOne(b => b.ApprovedByUser)
            .WithMany()
            .HasForeignKey(b => b.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Budget>()
            .HasOne(b => b.Organization)
            .WithMany()
            .HasForeignKey(b => b.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // BudgetLine Relationships
        modelBuilder.Entity<BudgetLine>()
            .HasOne(bl => bl.Budget)
            .WithMany(b => b.BudgetLines)
            .HasForeignKey(bl => bl.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Expense Relationships
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Project)
            .WithMany(p => p.Expenses)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.BudgetLine)
            .WithMany(bl => bl.Expenses)
            .HasForeignKey(e => e.BudgetLineId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.RecordedByUser)
            .WithMany(u => u.RecordedExpenses)
            .HasForeignKey(e => e.RecordedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification Relationships
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Organization)
            .WithMany(o => o.Notifications)
            .HasForeignKey(n => n.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // AuditLog Relationships
        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.Organization)
            .WithMany(o => o.AuditLogs)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private LambdaExpression CreateTenantFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(ITenantEntity.OrganizationId));

        var contextExpr = Expression.Constant(this);
        var currentOrgIdProp = Expression.Property(contextExpr, nameof(CurrentOrganizationId));
        var hasTenantProp = Expression.Property(contextExpr, nameof(HasTenant));

        var notHasTenant = Expression.Not(hasTenantProp);
        var equalsOrgId = Expression.Equal(
            Expression.Convert(property, typeof(int?)),
            currentOrgIdProp
        );

        var body = Expression.OrElse(notHasTenant, equalsOrgId);

        return Expression.Lambda(body, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentTenantId = _tenantService?.OrganizationId;
        var currentUserId = _currentUserService?.UserId;
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.OrganizationId == 0 && currentTenantId.HasValue)
            {
                entry.Entity.OrganizationId = currentTenantId.Value;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                if (currentUserId.HasValue && !entry.Entity.CreatedByUserId.HasValue)
                {
                    entry.Entity.CreatedByUserId = currentUserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                if (currentUserId.HasValue)
                {
                    entry.Entity.UpdatedByUserId = currentUserId;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}