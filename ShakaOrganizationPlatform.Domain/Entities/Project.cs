using ShakaOrganizationPlatform.Domain.Common;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
    public decimal ProgressPercentage { get; set; } = 0;
    public int? ProjectManagerId { get; set; }

    // Navigation Properties
    public Organization Organization { get; set; } = null!;
    public User? ProjectManager { get; set; }
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}