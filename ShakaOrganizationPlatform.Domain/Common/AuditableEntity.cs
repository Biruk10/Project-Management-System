namespace ShakaOrganizationPlatform.Domain.Common;

public abstract class AuditableEntity : TenantEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
}