namespace ShakaOrganizationPlatform.Domain.Common;

public abstract class TenantEntity : BaseEntity, ITenantEntity
{
    public int OrganizationId { get; set; }
}
