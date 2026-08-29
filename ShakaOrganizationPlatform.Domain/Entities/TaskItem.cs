using TaskStatus = ShakaOrganizationPlatform.Domain.Enums.TaskStatus;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class TaskItem : AuditableEntity
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssignedToUserId { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public DateTime? DueDate { get; set; }
    public decimal CompletionPercentage { get; set; } = 0;

    // Navigation Properties
    public Project Project { get; set; } = null!;
    public User? AssignedToUser { get; set; }
}