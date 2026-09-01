using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class UserRole : TenantEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public int? AssignedByUserId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
