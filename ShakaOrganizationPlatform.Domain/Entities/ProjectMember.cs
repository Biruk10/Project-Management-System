using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class ProjectMember : TenantEntity
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
