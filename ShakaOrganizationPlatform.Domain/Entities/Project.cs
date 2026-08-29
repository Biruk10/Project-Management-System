
namespace ShakaOrganizationPlatform.Domain.Entities;

public class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate {get; set;}
    public DateTime? EndDate {get; set;}
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;

    public decimal ProgressPercentage {get; set;} = 0;
    public int? ProjectManagerId { get; set;}
    
    public User? projectManager {get; set;}
}