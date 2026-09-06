using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class Permission : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
