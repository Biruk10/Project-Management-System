using ShakaOrganizationPlatform.Domain.Common;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class User : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    public Organization Organization { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    public ICollection<Expense> RecordedExpenses { get; set; } = new List<Expense>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}