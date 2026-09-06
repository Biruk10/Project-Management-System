using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class ProjectMember : TenantEntity
{
    public int ProjectId { get; set; }
    public int? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
    public User? User { get; set; }
    public Organization Organization { get; set; } = null!;
}
